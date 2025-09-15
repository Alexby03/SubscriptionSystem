namespace SubscriptionSystem.Events;

using SubscriptionSystem.Data;
using SubscriptionSystem.Services;

public class SubscriptionCreatedEventHandler : IEventHandler<SubscriptionCreatedEvent>
{
    private readonly AppDbContext _db;

    public SubscriptionCreatedEventHandler(AppDbContext db)
    {
        _db = db;
    }

    public Task HandleAsync(SubscriptionCreatedEvent @event)
    {
        Console.WriteLine($"Subscription Created: {@event.PlanId} ({@event.CustomerId})");
        return Task.CompletedTask;
    }
}
