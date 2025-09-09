namespace SubscriptionSystem.Results;

public enum UpgradeResult
{
    Success,
    SubscriptionInactive,
    AlreadyAtHighestPlan,
    PaymentMethodMissing
}