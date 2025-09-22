namespace SubscriptionSystem.Events;

using SubscriptionSystem.Data;
using SubscriptionSystem.Outbox;
using SubscriptionSystem.Services;

public class CustomerCreatedEventHandler : IEventHandler<CustomerCreatedEvent>
{
    private readonly AppDbContext _db;
    private readonly ILogger<OutboxWorker> _logger;

    public CustomerCreatedEventHandler(AppDbContext db, ILogger<OutboxWorker> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task HandleAsync(CustomerCreatedEvent @event)
    {
        _logger.LogInformation("EVENT HANDLED: Customer Created: {Name} ({Email})", @event.Name, @event.Email);
        return Task.CompletedTask;
    }
}
