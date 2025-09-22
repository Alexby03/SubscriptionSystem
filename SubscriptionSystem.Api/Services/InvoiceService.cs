namespace SubscriptionSystem.Services;

using SubscriptionSystem.Data;
using SubscriptionSystem.Entities;
using SubscriptionSystem.Results;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Enums;

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
}
