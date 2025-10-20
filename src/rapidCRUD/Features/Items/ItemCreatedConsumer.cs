using MassTransit;

namespace rapidCRUD.Features.Items;

public class ItemCreatedConsumer(ILogger<ItemCreatedConsumer> logger) : IConsumer<ItemCreatedEvent>
{
    public Task Consume(ConsumeContext<ItemCreatedEvent> context)
    {
        logger.LogInformation(
            "Item created event received: {ItemId} - {Name}",
            context.Message.ItemId,
            context.Message.Name);

        // Process the event (e.g., send notifications, update cache, etc.)
        
        return Task.CompletedTask;    
    }
}