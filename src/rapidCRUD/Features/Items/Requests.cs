using FluentValidation;

namespace rapidCRUD.Features.Items;

public class CreateItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateItemValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}

public record UpdateItemRequest(string Name, string? Description);

public class UpdateItemValidator : AbstractValidator<UpdateItemRequest>
{
    public UpdateItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public record GetItemRequest;
public record DeleteItemRequest;
public record GetItemsRequest;