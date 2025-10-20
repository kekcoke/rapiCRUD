namespace rapidCRUD.Features.Items;

public record ItemResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public ItemResponse(Item item)
    {
        Id = item.Id;
        Name = item.Name;
        Description = item.Description;
        CreatedAt = item.CreatedAt;
        UpdatedAt = item.UpdatedAt;
    }
}
