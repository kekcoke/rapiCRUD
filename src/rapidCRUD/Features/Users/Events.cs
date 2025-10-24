namespace rapidCRUD.Features.Users;

public record UserCreatedEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record UserUpdatedEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record UserDeletedEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public DateTime DeletedAt { get; init; }
}

public record UserLoginEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public DateTime LoginAt { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}