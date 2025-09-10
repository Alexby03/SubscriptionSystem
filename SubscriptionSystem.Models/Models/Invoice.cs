using SubscriptionSystem.Enums;
using SubscriptionSystem.Results;

namespace SubscriptionSystem.Models;

public class Invoice
{
    public Guid InvoiceId { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public DateTime IssueDate { get; private set; } = DateTime.UtcNow;
    public DateTime DueDate { get; private set; }
    public DateTime PaidDate { get; private set; }
    public decimal Amount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public Guid? PaymentId { get; private set; }

    public Invoice(DateTime dueDate, decimal amount, Guid customerId, Guid? payment = null)
    {
        DueDate = dueDate;
        Amount = amount;
        Status = InvoiceStatus.Pending;
        CustomerId = customerId;
        PaymentId = payment;
    }

    public PayInvoiceResult MarkPaid(Guid paymentId) //TODO
    {
        if (PaymentId != null) return PayInvoiceResult.AlreadyPaid;
        PaymentId = paymentId;
        PaidDate = DateTime.UtcNow;
        Status = InvoiceStatus.Paid;
        return PayInvoiceResult.Success;
    }

}