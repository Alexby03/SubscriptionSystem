namespace SubscriptionSystem.Results;

public enum ProcessPaymentResult
{
    Pending,
    Success,
    InvalidAmount,
    InvalidPaymentMethod,
    NetworkError,
    Declined,
    AlreadyProcessed,
    Refunded
}