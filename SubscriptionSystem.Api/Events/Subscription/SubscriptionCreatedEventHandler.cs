namespace SubscriptionSystem.Events;

using SubscriptionSystem.Data;
<<<<<<< HEAD
using SubscriptionSystem.Outbox;
=======
>>>>>>> 0817f30e54bf74e2fc41d86bdafd721a068df0ed
using SubscriptionSystem.Services;

public class SubscriptionCreatedEventHandler : IEventHandler<SubscriptionCreatedEvent>
{
    private readonly AppDbContext _db;
<<<<<<< HEAD
    private readonly ILogger<OutboxWorker> _logger;

    public SubscriptionCreatedEventHandler(AppDbContext db, ILogger<OutboxWorker> logger)
    {
        _db = db;
        _logger = logger;
=======

    public SubscriptionCreatedEventHandler(AppDbContext db)
    {
        _db = db;
>>>>>>> 0817f30e54bf74e2fc41d86bdafd721a068df0ed
    }

    public Task HandleAsync(SubscriptionCreatedEvent @event)
    {
<<<<<<< HEAD
        _logger.LogInformation("EVENT HANDLED: Subscription Created: {PlanId} ({CustomerId})", @event.PlanId, @event.CustomerId);
=======
        Console.WriteLine($"EVENT HANDLED: Subscription Created: {@event.PlanId} ({@event.CustomerId})");
>>>>>>> 0817f30e54bf74e2fc41d86bdafd721a068df0ed
        return Task.CompletedTask;
    }
}
