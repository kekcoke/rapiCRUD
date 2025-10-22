namespace rapidCRUD.Features.Users.Responses;

public class UserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string FullName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }

    public UserResponse() { }

    public UserResponse(User user)
    {
        Id = user.Id;
        Email = user.Email;
        Username = user.Username;
        FirstName = user.FirstName;
        LastName = user.LastName;
        FullName = user.FullName;
        IsActive = user.IsActive;
        CreatedAt = user.CreatedAt;
        UpdatedAt = user.UpdatedAt;
        LastLoginAt = user.LastLoginAt;
    }
}
