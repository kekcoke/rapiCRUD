using Microsoft.ApplicationInsights;
using rapidCRUD.Features.Shared;

namespace rapidCRUD.Features.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<List<User>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateLastLoginAsync(Guid userId, DateTime loginTime, CancellationToken cancellationToken = default);
}


public class UsersRepository : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<User>> GetPagedAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RegisterUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ActivateUser(Guid userId, string token)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUser(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUserPassword(Guid userId, string newPassword)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        throw new NotImplementedException();
    }


    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateLastLoginAsync(Guid userId, DateTime loginTime, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}