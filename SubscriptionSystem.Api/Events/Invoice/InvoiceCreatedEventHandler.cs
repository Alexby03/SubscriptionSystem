namespace SubscriptionSystem.Events;
public class InvoiceCreatedEventHandler : IEventHandler<InvoiceCreatedEvent>
{
    private readonly ILogger<InvoiceCreatedEventHandler> _logger;

    public InvoiceCreatedEventHandler(ILogger<InvoiceCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(InvoiceCreatedEvent @event)
    {
        _logger.LogInformation(
            "EVENT HANDLED: Invoice Created: InvoiceId={@event.InvoiceId}, CustomerId={@event.CustomerId}, Amount={@event.Amount:C}",
            @event.InvoiceId,
            @event.CustomerId,
            @event.Amount
        );
        return Task.CompletedTask;
    }
}
