namespace SubscriptionSystem.Events;

using SubscriptionSystem.Data;
using SubscriptionSystem.Services;

public class CustomerCreatedEventHandler : IEventHandler<CustomerCreatedEvent>
{
    private readonly AppDbContext _db;
    private readonly ILogger<CustomerCreatedEventHandler> _logger;

    public CustomerCreatedEventHandler(AppDbContext db, ILogger<CustomerCreatedEventHandler> Logger)
    {
        _db = db;
        _logger = Logger;
    }

    public Task HandleAsync(CustomerCreatedEvent @event)
    {
        _logger.LogInformation(
            "EVENT HANDLED: Customer Created: {@event.Name} ({@event.Email})",
            @event.Name,
            @event.Email
        );
        return Task.CompletedTask;
    }
}
