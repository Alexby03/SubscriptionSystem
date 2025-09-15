namespace SubscriptionSystem.Entities;

using SubscriptionSystem.Enums;
using SubscriptionSystem.Results;

public class Payment
{
    public Guid PaymentId { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime PaymentDate { get; private set; } = DateTime.UtcNow;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

    public Payment(decimal amount, Guid customerId, Guid invoiceId)
    {
        Amount = amount;
        CustomerId = customerId;
        InvoiceId = invoiceId;
    }

    public ProcessPaymentResult ProcessPayment()
    {
        Status = PaymentStatus.Paid;
        return ProcessPaymentResult.Success; //TODO
    }

    public ProcessPaymentResult Refund()
    {
        Status = PaymentStatus.Refunded;
        return ProcessPaymentResult.Refunded; //TODO
    }
}