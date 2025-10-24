using rapidCRUD.Features.Users;
using FluentAssertions;
using Xunit;
using rapidCRUD.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace  rapidCRUD.Tests.Unit;

public class UserRepositoryTests
{
    private ApplicationDbContext  GetInMemoryDbContext()  
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>();
            options.UseInMemoryDatabase(Guid.NewGuid().ToString());
        
        return new ApplicationDbContext(options.Options);
    }
}