namespace SubscriptionSystem.Services;

using SubscriptionSystem.Data;
using SubscriptionSystem.Entities;
using SubscriptionSystem.Results;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Enums;
using SubscriptionSystem.Events;
using SubscriptionSystem.Outbox;
using System.Text.Json;

public class InvoiceService
{
    private readonly AppDbContext _db;

    public InvoiceService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId)
    {
        return await _db.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
    }

    public async Task<PayInvoiceResult> MarkPaidAsync(Guid invoiceId)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        if (invoice == null) return PayInvoiceResult.NotFound;

        var result = invoice.MarkPaid();
        if (result == PayInvoiceResult.Success)
        {
            await _db.SaveChangesAsync();
            Console.WriteLine($"Invoice {invoice.InvoiceId} marked as paid for Customer {invoice.CustomerId}, Amount={invoice.Amount:C}");
        }
        return result;
    }

    public async Task<List<Invoice>> GetInvoicesForCustomerAsync(Guid customerId)
    {
        return await _db.Invoices.Where(i => i.CustomerId == customerId).ToListAsync();
    }

    public async Task<List<Invoice>> GetUnpaidInvoicesForCustomerAsync(Guid customerId)
    {
        return await _db.Invoices.Where(i => i.CustomerId == customerId && i.Status != InvoiceStatus.Paid).ToListAsync();
    }

    public async Task<Invoice> GenerateInvoiceAsync(
        Guid customerId,
        Guid subscriptionId,
        decimal amount,
        DateTime servicePeriodStart,
        DateTime servicePeriodEnd)
    {
        var invoice = new Invoice(
            customerId: customerId,
            amount: amount,
            dueDate: servicePeriodEnd, // due at end of service period
            subscriptionId: subscriptionId,
            servicePeriodStart: servicePeriodStart,
            servicePeriodEnd: servicePeriodEnd
        );

        _db.Invoices.Add(invoice);

        var invoiceCreatedEvent = new InvoiceCreatedEvent(invoice.InvoiceId, invoice.CustomerId, invoice.Amount);
        var outboxEvent = new OutboxEvent
        {
            EventType = nameof(InvoiceCreatedEvent),
            Payload = JsonSerializer.Serialize(invoiceCreatedEvent)
        };
        _db.OutboxEvents.Add(outboxEvent);

        await _db.SaveChangesAsync();

        Console.WriteLine($"Generated invoice for Customer {customerId}, Amount={amount:C}, ServicePeriod={servicePeriodStart:yyyy-MM-dd} to {servicePeriodEnd:yyyy-MM-dd}");
        return invoice;
    }
}
