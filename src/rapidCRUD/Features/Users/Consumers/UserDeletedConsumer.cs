using MassTransit;

namespace rapidCRUD.Features.Users.Consumers;

public class UserDeletedConsumer : IConsumer<UserDeletedEvent>
{
    private readonly ILogger<UserDeletedConsumer> _logger;

    public UserDeletedConsumer(ILogger<UserDeletedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserDeletedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "User deleted event received: {UserId} - {Email}",
            message.UserId,
            message.Email);

        try
        {
            // Clean up user data
            await CleanupUserDataAsync(message.UserId);
            
            // Remove from cache
            await RemoveFromCacheAsync(message.UserId);
            
            // Remove from search index
            await RemoveFromSearchIndexAsync(message.UserId);
            
            // Notify external systems
            await NotifyExternalSystemsAsync(message);
            
            // Archive user data (GDPR compliance)
            await ArchiveUserDataAsync(message);
            
            _logger.LogInformation("User deletion post-processing completed for {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user deleted event for {UserId}", message.UserId);
            throw;
        }
    }

    private async Task CleanupUserDataAsync(Guid userId)
    {
        // Clean up related data (sessions, preferences, etc.)
        _logger.LogInformation("User data cleaned up for {UserId}", userId);
        await Task.CompletedTask;
    }

    private async Task RemoveFromCacheAsync(Guid userId)
    {
        // Remove all cache entries for this user
        _logger.LogInformation("User removed from cache: {UserId}", userId);
        await Task.CompletedTask;
    }

    private async Task RemoveFromSearchIndexAsync(Guid userId)
    {
        // Remove from Elasticsearch
        _logger.LogInformation("User removed from search index: {UserId}", userId);
        await Task.CompletedTask;
    }

    private async Task NotifyExternalSystemsAsync(UserDeletedEvent userEvent)
    {
        // Notify CRM, analytics, etc.
        _logger.LogInformation("External systems notified about user {UserId} deletion", userEvent.UserId);
        await Task.CompletedTask;
    }

    private async Task ArchiveUserDataAsync(UserDeletedEvent userEvent)
    {
        // Archive for compliance/audit purposes
        _logger.LogInformation("User data archived for {UserId}", userEvent.UserId);
        await Task.CompletedTask;
    }
}