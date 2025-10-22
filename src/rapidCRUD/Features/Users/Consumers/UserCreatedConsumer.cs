using MassTransit;

namespace rapidCRUD.Features.Users.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedConsumer> _logger;
    private readonly IUserRepository _userRepository;

    public UserCreatedConsumer(
        ILogger<UserCreatedConsumer> logger,
        IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "User created event received: {UserId} - {Email} - {Username}",
            message.UserId,
            message.Email,
            message.Username);

        // Perform post-creation tasks
        // Example: Send welcome email, create default settings, notify admins, etc.
        
        try
        {
            // Send welcome email (placeholder)
            await SendWelcomeEmailAsync(message);
            
            // Create user profile or default settings
            await CreateDefaultUserSettingsAsync(message.UserId);
            
            // Log to analytics
            await LogUserCreationAnalyticsAsync(message);
            
            _logger.LogInformation("User creation post-processing completed for {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user created event for {UserId}", message.UserId);
            throw; // Will trigger retry via MassTransit
        }
    }

    private async Task SendWelcomeEmailAsync(UserCreatedEvent userEvent)
    {
        // Implement email sending logic
        _logger.LogInformation("Welcome email sent to {Email}", userEvent.Email);
        await Task.CompletedTask;
    }

    private async Task CreateDefaultUserSettingsAsync(Guid userId)
    {
        // Create default user settings, preferences, etc.
        _logger.LogInformation("Default settings created for user {UserId}", userId);
        await Task.CompletedTask;
    }

    private async Task LogUserCreationAnalyticsAsync(UserCreatedEvent userEvent)
    {
        // Log to analytics platform
        _logger.LogInformation("User creation logged to analytics: {UserId}", userEvent.UserId);
        await Task.CompletedTask;
    }
}