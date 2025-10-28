using SubscriptionSystem.Enums;
using SubscriptionSystem.Results;

namespace SubscriptionSystem.Entities;

public class Invoice
{
    public Guid InvoiceId { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public Guid? SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    public DateTime IssueDate { get; private set; } = DateTime.UtcNow;
    public DateTime DueDate { get; private set; }
    public DateTime? ServicePeriodStart { get; private set; }
    public DateTime? ServicePeriodEnd { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public decimal Amount { get; private set; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Pending;

    private Invoice() { }

    public Invoice(
        Guid customerId,
        decimal amount,
        DateTime dueDate,
        Guid? subscriptionId = null,
        DateTime? servicePeriodStart = null,
        DateTime? servicePeriodEnd = null)
    {
        CustomerId = customerId;
        Amount = amount;
        DueDate = dueDate;
        SubscriptionId = subscriptionId;
        ServicePeriodStart = servicePeriodStart;
        ServicePeriodEnd = servicePeriodEnd;
    }

    public PayInvoiceResult MarkPaid()
    {
        if (Status == InvoiceStatus.Paid)
            return PayInvoiceResult.AlreadyPaid;

        PaidDate = DateTime.UtcNow;
        Status = InvoiceStatus.Paid;
        return PayInvoiceResult.Success;
    }

    public void MarkOverdueIfPastDue()
    {
        if (Status == InvoiceStatus.Pending && DateTime.UtcNow > DueDate)
            Status = InvoiceStatus.Overdue;
    }

    public GenericResult Cancel()
    {
        if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Overdue)
            return GenericResult.Failed;

        Status = InvoiceStatus.Canceled;
        return GenericResult.Success;
    }
}
