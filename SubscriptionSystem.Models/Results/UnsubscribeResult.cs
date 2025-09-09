namespace SubscriptionSystem.Results;

public enum UnsubscribeResult
{
    Success,
    NotFound,
    AlreadyCanceled,
    AccountSuspended,
    PaymentIssue
}
