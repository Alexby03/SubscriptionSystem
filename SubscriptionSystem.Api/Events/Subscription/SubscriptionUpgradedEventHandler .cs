namespace SubscriptionSystem.Events;

public class SubscriptionUpgradedEventHandler : IEventHandler<SubscriptionUpgradedEvent>
{
    private readonly ILogger<SubscriptionUpgradedEventHandler> _logger;

    public SubscriptionUpgradedEventHandler(ILogger<SubscriptionUpgradedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(SubscriptionUpgradedEvent @event)
    {
        _logger.LogInformation(
            "Subscription {@event.SubscriptionId} upgraded from Plan {@event.OldPlanId} to {@event.NewPlanId}. Prorated amount: {@event.Amount}",
            @event.SubscriptionId, @event.OldPlanId, @event.NewPlanId, @event.ProratedAmount
        );
        return Task.CompletedTask;
    }
}
