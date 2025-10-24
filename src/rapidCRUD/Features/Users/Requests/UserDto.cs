using rapidCRUD.Features.Shared;

namespace rapidCRUD.Features.Users.Requests;

public abstract record CreateUserDto
{
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public abstract record UpdateUserDto
{
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public bool IsActive { get; init; }
}

public record CreateUserRequest : CreateRequest<CreateUserDto>;
public record UpdateUserRequest : UpdateRequest<UpdateUserDto>;