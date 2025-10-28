namespace SubscriptionSystem.Events;

using SubscriptionSystem.Data;
using SubscriptionSystem.Services;

public class SubscriptionCreatedEventHandler : IEventHandler<SubscriptionCreatedEvent>
{
    private readonly AppDbContext _db;
    private readonly ILogger<SubscriptionCreatedEventHandler> _logger;

    public SubscriptionCreatedEventHandler(AppDbContext db, ILogger<SubscriptionCreatedEventHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task HandleAsync(SubscriptionCreatedEvent @event)
    {
        _logger.LogInformation(
            "EVENT HANDLED: Subscription Created: {@event.PlanId} ({@event.CustomerId})",
            @event.PlanId,
            @event.CustomerId
        );
        return Task.CompletedTask;
    }
}
