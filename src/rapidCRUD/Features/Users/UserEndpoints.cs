using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using rapidCRUD.Features.Shared;
using System.Security.Claims;
using rapidCRUD.Features.Users.Requests;
using rapidCRUD.Features.Users.Responses;
using rapidCRUD.Features.Users.Validators;

namespace rapidCRUD.Features.Users;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users");

        // Public endpoints
        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithOpenApi()
            .AllowAnonymous(); // Typically for registration

        // Protected endpoints
        group.MapGet("/{id:guid}", GetUser)
            .WithName("GetUser")
            .WithOpenApi()
            .RequireAuthorization("UserAccess");

        group.MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithOpenApi()
            .RequireAuthorization("UserAccess");

        group.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithOpenApi()
            .RequireAuthorization("UserAccess");

        group.MapDelete("/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithOpenApi()
            .RequireAuthorization("AdminOnly");

        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithOpenApi()
            .RequireAuthorization("UserAccess");

        group.MapPut("/me", UpdateCurrentUser)
            .WithName("UpdateCurrentUser")
            .WithOpenApi()
            .RequireAuthorization("UserAccess");

        group.MapGet("/email/{email}", GetUserByEmail)
            .WithName("GetUserByEmail")
            .WithOpenApi()
            .RequireAuthorization("AdminOnly");

        group.MapGet("/username/{username}", GetUserByUsername)
            .WithName("GetUserByUsername")
            .WithOpenApi()
            .RequireAuthorization("AdminOnly");

        group.MapPost("/{id:guid}/login", RecordLogin)
            .WithName("RecordLogin")
            .WithOpenApi()
            .RequireAuthorization("UserAccess");
    }

    private static async Task<IResult> CreateUser(
        [FromBody] CreateUserRequest request,
        [FromServices] IUserRepository repository,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] ILogger<CreateUserRequest> logger,
        HttpContext httpContext)
    {
        var validator = new CreateUserValidator();
        var validationResult = await validator.ValidateAsync(request.Data);
        
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        // Check if email already exists
        if (await repository.EmailExistsAsync(request.Data.Email))
        {
            return Results.Conflict(Shared.Response<UserResponse>.Fail("Email already exists"));
        }

        // Check if username already exists
        if (await repository.UsernameExistsAsync(request.Data.Username))
        {
            return Results.Conflict(Shared.Response<UserResponse>.Fail("Username already exists"));
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Data.Email,
            Username = request.Data.Username,
            FirstName = request.Data.FirstName,
            LastName = request.Data.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(user);
        
        // Publish event
        await publishEndpoint.Publish(new UserCreatedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        });

        logger.LogInformation("User created: {UserId} - {Email}", user.Id, user.Email);

        return Results.Created(
            $"/api/users/{user.Id}", 
            Shared.Response<UserResponse>.Ok(new UserResponse(user), "User created successfully"));
    }

    private static async Task<IResult> GetUser(
        Guid id,
        [FromServices] IUserRepository repository,
        [FromServices] ILogger<User> logger,
        ClaimsPrincipal user)
    {
        var requestingUserId = GetUserIdFromClaims(user);
        var isAdmin = user.IsInRole("admin");

        var foundUser = await repository.GetByIdAsync(id);
        
        if (foundUser == null)
        {
            return Results.NotFound(Shared.Response<UserResponse>.Fail("User not found"));
        }

        // Users can only view their own profile unless they're admin
        if (!isAdmin && requestingUserId != id)
        {
            return Results.Forbid();
        }

        return Results.Ok(Shared.Response<UserResponse>.Ok(new UserResponse(foundUser)));
    }

    private static async Task<IResult> GetUsers(
        [AsParameters] PagedRequest request,
        [FromServices] IUserRepository repository,
        ClaimsPrincipal user)
    {
        if (!user.IsInRole("admin"))
        {
            return Results.Forbid();
        }

        var users = await repository.GetPagedAsync(request);
        var totalCount = await repository.CountAsync(request.SearchTerm);

        var response = new PagedResponse<UserResponse>
        {
            Items = users.Select(u => new UserResponse(u)).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        return Results.Ok(response);
    }

    private static async Task<IResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        [FromServices] IUserRepository repository,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] ILogger<User> logger,
        ClaimsPrincipal user)
    {
        var requestingUserId = GetUserIdFromClaims(user);
        var isAdmin = user.IsInRole("admin");

        // Users can only update their own profile unless they're admin
        if (!isAdmin && requestingUserId != id)
        {
            return Results.Forbid();
        }

        var validator = new UpdateUserValidator();
        var validationResult = await validator.ValidateAsync(request.Data);
        
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var existingUser = await repository.GetByIdAsync(id);
        
        if (existingUser == null)
        {
            return Results.NotFound(Shared.Response<UserResponse>.Fail("User not found"));
        }

        // Check if email already exists (excluding current user)
        if (await repository.EmailExistsAsync(request.Data.Email, id))
        {
            return Results.Conflict(Shared.Response<UserResponse>.Fail("Email already exists"));
        }

        // Check if username already exists (excluding current user)
        if (await repository.UsernameExistsAsync(request.Data.Username, id))
        {
            return Results.Conflict(Shared.Response<UserResponse>.Fail("Username already exists"));
        }

        existingUser.Email = request.Data.Email;
        existingUser.Username = request.Data.Username;
        existingUser.FirstName = request.Data.FirstName;
        existingUser.LastName = request.Data.LastName;
        
        // Only admins can change IsActive status
        if (isAdmin)
        {
            existingUser.IsActive = request.Data.IsActive;
        }
        
        existingUser.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(existingUser);

        await publishEndpoint.Publish(new UserUpdatedEvent
        {
            UserId = existingUser.Id,
            Email = existingUser.Email,
            Username = existingUser.Username,
            FirstName = existingUser.FirstName,
            LastName = existingUser.LastName,
            UpdatedAt = existingUser.UpdatedAt
        });

        logger.LogInformation("User updated: {UserId}", existingUser.Id);

        return Results.Ok(Shared.Response<UserResponse>.Ok(new UserResponse(existingUser), "User updated successfully"));
    }

    private static async Task<IResult> DeleteUser(
        Guid id,
        [FromServices] IUserRepository repository,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] ILogger<User> logger)
    {
        var user = await repository.GetByIdAsync(id);
        
        if (user == null)
        {
            return Results.NotFound(Shared.Response<UserResponse>.Fail("User not found"));
        }

        await repository.DeleteAsync(user);

        await publishEndpoint.Publish(new UserDeletedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            DeletedAt = DateTime.UtcNow
        });

        logger.LogInformation("User deleted: {UserId}", user.Id);

        return Results.NoContent();
    }

    private static async Task<IResult> GetCurrentUser(
        [FromServices] IUserRepository repository,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        
        if (userId == Guid.Empty)
        {
            return Results.Unauthorized();
        }

        var currentUser = await repository.GetByIdAsync(userId);
        
        if (currentUser == null)
        {
            return Results.NotFound(Shared.Response<UserResponse>.Fail("User not found"));
        }

        return Results.Ok(Shared.Response<UserResponse>.Ok(new UserResponse(currentUser)));
    }

    private static async Task<IResult> UpdateCurrentUser(
        [FromBody] UpdateUserRequest request,
        [FromServices] IUserRepository repository,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] ILogger<User> logger,
        ClaimsPrincipal user)
    {
        var userId = GetUserIdFromClaims(user);
        
        if (userId == Guid.Empty)
        {
            return Results.Unauthorized();
        }

        return await UpdateUser(userId, request, repository, publishEndpoint, logger, user);
    }

    private static async Task<IResult> GetUserByEmail(
        string email,
        [FromServices] IUserRepository repository)
    {
        var user = await repository.GetByEmailAsync(email);
        
        if (user == null)
        {
            return Results.NotFound(Shared.Response<UserResponse>.Fail("User not found"));
        }

        return Results.Ok(Shared.Response<UserResponse>.Ok(new UserResponse(user)));
    }

    private static async Task<IResult> GetUserByUsername(
        string username,
        [FromServices] IUserRepository repository)
    {
        var user = await repository.GetByUsernameAsync(username);
        
        if (user == null)
        {
            return Results.NotFound(Shared.Response<UserResponse>.Fail("User not found"));
        }

        return Results.Ok(Shared.Response<UserResponse>.Ok(new UserResponse(user)));
    }

    private static async Task<IResult> RecordLogin(
        Guid id,
        [FromServices] IUserRepository repository,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] ILogger<User> logger,
        HttpContext httpContext,
        ClaimsPrincipal user)
    {
        var requestingUserId = GetUserIdFromClaims(user);

        // Users can only record their own login
        if (requestingUserId != id && !user.IsInRole("admin"))
        {
            return Results.Forbid();
        }

        var existingUser = await repository.GetByIdAsync(id);
        
        if (existingUser == null)
        {
            return Results.NotFound(Shared.Response<UserResponse>.Fail("User not found"));
        }

        var loginTime = DateTime.UtcNow;
        await repository.UpdateLastLoginAsync(id, loginTime);

        // Get IP and User Agent
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

        await publishEndpoint.Publish(new UserLoginEvent
        {
            UserId = id,
            Email = existingUser.Email,
            LoginAt = loginTime,
            IpAddress = ipAddress,
            UserAgent = userAgent
        });

        logger.LogInformation("Login recorded for user {UserId} from {IpAddress}", id, ipAddress);

        return Results.Ok(Shared.Response<object>.Ok(new { lastLoginAt = loginTime }, "Login recorded successfully"));
    }

    private static Guid GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) 
                       ?? user.FindFirst("sub")
                       ?? user.FindFirst("user_id");
        
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return Guid.Empty;
    }
}