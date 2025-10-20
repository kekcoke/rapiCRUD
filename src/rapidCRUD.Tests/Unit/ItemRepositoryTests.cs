using rapidCRUD.Features.Items;
using FluentAssertions;
using Xunit;
using rapidCRUD.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace rapidCRUD.Tests.Unit;

[Trait("Category", "Unit")]
public class ItemRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>();
            options.UseInMemoryDatabase(Guid.NewGuid().ToString());
        
        return new ApplicationDbContext(options.Options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddItemToDatabase()
    {
        await using var context = GetInMemoryDbContext();
        var repository = new ItemRepository(context);
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.Now
        };
        
        await repository.AddAsync(item);
        var result = await repository.GetByIdAsync(item.Id);
        
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Item");
    }
}