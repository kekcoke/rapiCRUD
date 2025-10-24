using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using rapidCRUD.Features.Items.Validators;

namespace rapidCRUD.Features.Items;

public static class ItemEndpoints
{
    public static void MapItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/items")
            .WithTags("Items")
            .RequireAuthorization("UserAccess");

        group.MapPost("/", CreateItem)
            .WithName("CreateItem")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetItem)
            .WithName("GetItem")
            .WithOpenApi();

        group.MapPut("/{id:guid}", UpdateItem)
            .WithName("UpdateItem")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", DeleteItem)
            .WithName("DeleteItem")
            .RequireAuthorization("AdminOnly")
            .WithOpenApi();

        group.MapGet("/", GetItems)
            .WithName("GetItems")
            .WithOpenApi();
    }

    private static async Task<IResult> CreateItem(
        [FromBody] CreateItemRequest request,
        [FromServices] IItemRepository repository,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] ILogger<CreateItemRequest> logger)
    {
        var validator = new CreateItemValidator();
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(item);
        
        // Publish event
        await publishEndpoint.Publish(new ItemCreatedEvent
        {
            ItemId = item.Id,
            Name = item.Name,
            CreatedAt = item.CreatedAt
        });

        logger.LogInformation("Created item {ItemId}", item.Id);

        return Results.Created($"/api/items/{item.Id}", new ItemResponse(item));
    }

    private static async Task<IResult> GetItem(
        Guid id,
        [FromServices] IItemRepository repository,
        [FromServices] ILogger<GetItemRequest> logger)
    {
        var item = await repository.GetByIdAsync(id);
        
        if (item == null)
        {
            logger.LogWarning("Item {ItemId} not found", id);
            return Results.NotFound(new { Message = "Item not found" });
        }

        return Results.Ok(new ItemResponse(item));
    }

    private static async Task<IResult> UpdateItem(
        Guid id,
        [FromBody] UpdateItemRequest request,
        [FromServices] IItemRepository repository,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] ILogger<UpdateItemRequest> logger)
    {
        var validator = new UpdateItemValidator();
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var item = await repository.GetByIdAsync(id);
        
        if (item == null)
        {
            return Results.NotFound(new { Message = "Item not found" });
        }

        item.Name = request.Name;
        item.Description = request.Description;
        item.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(item);

        await publishEndpoint.Publish(new ItemUpdatedEvent
        {
            ItemId = item.Id,
            Name = item.Name,
            UpdatedAt = item.UpdatedAt
        });

        logger.LogInformation("Updated item {ItemId}", item.Id);

        return Results.Ok(new ItemResponse(item));
    }

    private static async Task<IResult> DeleteItem(
        Guid id,
        [FromServices] IItemRepository repository,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] ILogger<DeleteItemRequest> logger)
    {
        var item = await repository.GetByIdAsync(id);
        
        if (item == null)
        {
            return Results.NotFound(new { Message = "Item not found" });
        }

        await repository.DeleteAsync(item);

        await publishEndpoint.Publish(new ItemDeletedEvent
        {
            ItemId = item.Id,
            DeletedAt = DateTime.UtcNow
        });

        logger.LogInformation("Deleted item {ItemId}", item.Id);

        return Results.NoContent();
    }

    private static async Task<IResult> GetItems(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromServices] IItemRepository repository)
    {
        var items = await repository.GetPagedAsync(page, pageSize);
        var total = await repository.CountAsync();

        var response = new PagedResponse<ItemResponse>
        {
            Items = items.Select(i => new ItemResponse(i)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };

        return Results.Ok(response);
    }
}