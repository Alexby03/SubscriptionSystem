namespace SubscriptionSystem.Events;
public record SubscriptionUpgradedEvent(Guid SubscriptionId, int OldPlanId, int NewPlanId, decimal ProratedAmount) : IEvent;