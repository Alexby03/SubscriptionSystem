namespace SubscriptionSystem.Events;
public record SubscriptionBillingAdvancedEvent(Guid SubscriptionId, DateTime NewBillingDate) : IEvent;