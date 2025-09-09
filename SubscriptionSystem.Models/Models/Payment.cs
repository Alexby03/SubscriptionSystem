namespace SubscriptionSystem.Models;

using SubscriptionSystem.Enums;
using SubscriptionSystem.Results;

public class Payment
{
    public Guid GuId { get; private set; } = Guid.NewGuid();
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public PaymentStatus Status { get; private set; }


    public Payment(decimal amount, DateTime date, PaymentStatus status)
    {
        Amount = amount;
        Date = date;
        Status = status;
    }

    public ProcessPaymentResult ProcessPayment()
    {
        return ProcessPaymentResult.Success; //TODO
    }

    public ProcessPaymentResult Refund()
    {
        return ProcessPaymentResult.Refunded; //TODO
    }
}