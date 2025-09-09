using SubscriptionSystem.Enums;

namespace SubscriptionSystem.Models;

public class Invoice
{
    public Guid GuId { get; private set; } = Guid.NewGuid();
    public DateTime Date { get; private set; }
    public decimal Amount { get; private set; }
    public InvoiceStatus Status { get; private set; } 

    public Invoice(DateTime date, decimal amount)
    {
        Date = date;
        Amount = amount;
        Status = InvoiceStatus.Pending;
    }

    public InvoiceStatus MarkPaid()
    {
        Status = InvoiceStatus.Paid;
        return Status; //TODO
    }

}