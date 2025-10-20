namespace rapidCRUD.Features.Items;

public record ItemCreatedEvent
{
    public Guid ItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record ItemUpdatedEvent
{
    public Guid ItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
}

public record ItemDeletedEvent
{
    public Guid ItemId { get; init; }
    public DateTime DeletedAt { get; init; }
}