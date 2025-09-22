namespace SubscriptionSystem.Events;

using SubscriptionSystem.Data;
using SubscriptionSystem.Outbox;
using SubscriptionSystem.Services;

public class SubscriptionCreatedEventHandler : IEventHandler<SubscriptionCreatedEvent>
{
    private readonly AppDbContext _db;
    private readonly ILogger<OutboxWorker> _logger;

    public SubscriptionCreatedEventHandler(AppDbContext db, ILogger<OutboxWorker> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task HandleAsync(SubscriptionCreatedEvent @event)
    {
        _logger.LogInformation("EVENT HANDLED: Subscription Created: {PlanId} ({CustomerId})", @event.PlanId, @event.CustomerId);
        return Task.CompletedTask;
    }
}
