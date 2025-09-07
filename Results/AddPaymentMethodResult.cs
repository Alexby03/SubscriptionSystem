namespace SubscriptionSystem.Results;

public enum AddPaymentMethodResult
{
    Success,
    Duplicate,
    InvalidCard,
    ExpiredCard,
    PaymentNetworkError
}
