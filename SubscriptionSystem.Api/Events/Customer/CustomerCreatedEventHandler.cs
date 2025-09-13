namespace SubscriptionSystem.Events;

using SubscriptionSystem.Data;
using SubscriptionSystem.Services;

public class CustomerCreatedEventHandler : IEventHandler<CustomerCreatedEvent>
{
    private readonly AppDbContext _db;

    public CustomerCreatedEventHandler(AppDbContext db)
    {
        _db = db;
    }

    public Task HandleAsync(CustomerCreatedEvent @event)
    {
        // Example: Log to console for now
        Console.WriteLine($"Customer Created: {@event.Name} ({@event.Email})");

        // TODO: Could enqueue a welcome email, send to Azure Service Bus, etc.
        return Task.CompletedTask;
    }
}
