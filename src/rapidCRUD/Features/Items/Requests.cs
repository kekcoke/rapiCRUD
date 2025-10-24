using FluentValidation;

namespace rapidCRUD.Features.Items;

public class CreateItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}



public record GetItemRequest;
public record DeleteItemRequest;
public record GetItemsRequest;