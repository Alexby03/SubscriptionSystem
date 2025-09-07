namespace SubscriptionSystem.Results;

public enum SubscribeResult
{
    Success,
    AlreadySubscribed,
    InvalidPlan,
    PaymentFailed,
    AccountSuspended
}