namespace SubscriptionSystem.Events;
public record SubscriptionCreatedEvent(Guid CustomerId, int PlanId) : IEvent;
