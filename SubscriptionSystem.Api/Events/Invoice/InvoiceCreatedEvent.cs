namespace SubscriptionSystem.Events;
public record InvoiceCreatedEvent(Guid InvoiceId, Guid CustomerId, decimal Amount) : IEvent;