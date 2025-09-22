namespace SubscriptionSystem.Events;
public record CustomerCreatedEvent(Guid CustomerId, string Name, string Email) : IEvent;
