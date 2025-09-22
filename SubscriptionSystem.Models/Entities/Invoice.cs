using SubscriptionSystem.Enums;
using SubscriptionSystem.Results;

namespace SubscriptionSystem.Entities;

public class Invoice
{
    public Guid InvoiceId { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public DateTime IssueDate { get; private set; } = DateTime.UtcNow;
    public DateTime DueDate { get; private set; }
    public DateTime PaidDate { get; private set; }
    public decimal Amount { get; private set; }
    public InvoiceStatus Status { get; set; }

    public Invoice(DateTime dueDate, decimal amount, Guid customerId, Guid subscriptionId)
    {
        DueDate = dueDate;
        Amount = amount;
        Status = InvoiceStatus.Pending;
        CustomerId = customerId;
        SubscriptionId = subscriptionId;
    }

    public PayInvoiceResult MarkPaid()
    {
        if (Status == InvoiceStatus.Paid) return PayInvoiceResult.AlreadyPaid;
        PaidDate = DateTime.UtcNow;
        Status = InvoiceStatus.Paid;
        return PayInvoiceResult.Success;
    }
}