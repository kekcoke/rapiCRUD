using FluentAssertions;
using rapidCRUD.Features.Items;

namespace rapidCRUD.Tests.Unit;

[Trait("Category", "Unit")]
public class ValidationTests
{
    [Fact]
    public async Task CreateItemValidator_ShouldFail_OnEmptyName()
    {
        var validator = new CreateItemValidator();
        var request = new CreateItemRequest()
        {
            Name = "",
            Description = ""
        };

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateItemValidator_ShouldFail_OnEmptyDescription()
    {
        var validator = new CreateItemValidator();
        var request = new CreateItemRequest()
        {
            Name = "Valid Name",
            Description = ""
        };
        
        var result = await validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task CreateItemValidator_ShouldFail_WhenNameIsTooLong()
    {
        // Arrange
        var validator = new CreateItemValidator();
        var request = new CreateItemRequest()
        {
            Name = new string('a', 101),
            Description = "Valid Description"
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
    }
    
    [Fact]
    public async Task CreateItemValidator_ShouldFail_WhenDescriptionIsTooLong()
    {
        // Arrange
        var validator = new CreateItemValidator();
        var request = new CreateItemRequest()
        {
            Name = "Valid Name",
            Description = new string('a', 501)
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Description");
    }
    
    [Fact]
    public async Task CreateItemValidator_ShouldPass_OnValidInput()
    {
        var validator = new CreateItemValidator();
        var request = new CreateItemRequest()
        {
            Name = "Valid Name",
            Description = "Valid Description"
        };
        
        var result = await validator.ValidateAsync(request);
        
        result.IsValid.Should().BeTrue();
    }
}