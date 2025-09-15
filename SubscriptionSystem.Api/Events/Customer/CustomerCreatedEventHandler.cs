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
        Console.WriteLine($"EVENT HANDLED: Customer Created: {@event.Name} ({@event.Email})");
        return Task.CompletedTask;
    }
}
