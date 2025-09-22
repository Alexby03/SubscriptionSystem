namespace SubscriptionSystem.Events;

using SubscriptionSystem.Data;
<<<<<<< HEAD
using SubscriptionSystem.Outbox;
=======
>>>>>>> 0817f30e54bf74e2fc41d86bdafd721a068df0ed
using SubscriptionSystem.Services;

public class CustomerCreatedEventHandler : IEventHandler<CustomerCreatedEvent>
{
    private readonly AppDbContext _db;
<<<<<<< HEAD
    private readonly ILogger<OutboxWorker> _logger;

    public CustomerCreatedEventHandler(AppDbContext db, ILogger<OutboxWorker> logger)
    {
        _db = db;
        _logger = logger;
=======

    public CustomerCreatedEventHandler(AppDbContext db)
    {
        _db = db;
>>>>>>> 0817f30e54bf74e2fc41d86bdafd721a068df0ed
    }

    public Task HandleAsync(CustomerCreatedEvent @event)
    {
<<<<<<< HEAD
        _logger.LogInformation("EVENT HANDLED: Customer Created: {Name} ({Email})", @event.Name, @event.Email);
=======
        Console.WriteLine($"EVENT HANDLED: Customer Created: {@event.Name} ({@event.Email})");
>>>>>>> 0817f30e54bf74e2fc41d86bdafd721a068df0ed
        return Task.CompletedTask;
    }
}
