namespace SubscriptionSystem.Events;

public class SubscriptionBillingAdvancedEventHandler : IEventHandler<SubscriptionBillingAdvancedEvent>
{
    private readonly ILogger<SubscriptionBillingAdvancedEventHandler> _logger;

    public SubscriptionBillingAdvancedEventHandler(ILogger<SubscriptionBillingAdvancedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(SubscriptionBillingAdvancedEvent @event)
    {
        _logger.LogInformation(
            "Subscription {@event.SubscriptionId} has a new expiration date of: {@event.NewBillingDate}",
            @event.SubscriptionId, @event.NewBillingDate
        );
        return Task.CompletedTask;
    }
}