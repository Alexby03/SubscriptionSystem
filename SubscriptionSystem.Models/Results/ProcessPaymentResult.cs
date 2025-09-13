namespace SubscriptionSystem.Results;

public enum ProcessPaymentResult
{
    Pending,
    Success,
    InvalidAmount,
    NetworkError,
    Declined,
    AlreadyProcessed,
    Refunded,
    NotFound,
    Failed
}