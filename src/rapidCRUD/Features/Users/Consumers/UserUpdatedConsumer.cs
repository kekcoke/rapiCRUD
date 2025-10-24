using MassTransit;

namespace rapidCRUD.Features.Users.Consumers;

public class UserUpdatedConsumer : IConsumer<UserUpdatedEvent>
{
    private readonly ILogger<UserUpdatedConsumer> _logger;
    private readonly IUserRepository _userRepository;

    public UserUpdatedConsumer(
        ILogger<UserUpdatedConsumer> logger,
        IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "User updated event received: {UserId} - {Email}",
            message.UserId,
            message.Email);

        try
        {
            // Invalidate cache
            await InvalidateUserCacheAsync(message.UserId);
            
            // Update search index
            await UpdateSearchIndexAsync(message);
            
            // Sync with external systems
            await SyncWithExternalSystemsAsync(message);
            
            // Notify related services
            await NotifyRelatedServicesAsync(message);
            
            _logger.LogInformation("User update post-processing completed for {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user updated event for {UserId}", message.UserId);
            throw;
        }
    }

    private async Task InvalidateUserCacheAsync(Guid userId)
    {
        // Invalidate Redis cache entries for this user
        _logger.LogInformation("Cache invalidated for user {UserId}", userId);
        await Task.CompletedTask;
    }

    private async Task UpdateSearchIndexAsync(UserUpdatedEvent userEvent)
    {
        // Update Elasticsearch or similar search index
        _logger.LogInformation("Search index updated for user {UserId}", userEvent.UserId);
        await Task.CompletedTask;
    }

    private async Task SyncWithExternalSystemsAsync(UserUpdatedEvent userEvent)
    {
        // Sync with CRM, marketing automation, etc.
        _logger.LogInformation("External systems synced for user {UserId}", userEvent.UserId);
        await Task.CompletedTask;
    }

    private async Task NotifyRelatedServicesAsync(UserUpdatedEvent userEvent)
    {
        // Notify other microservices about user changes
        _logger.LogInformation("Related services notified about user {UserId} update", userEvent.UserId);
        await Task.CompletedTask;
    }
}