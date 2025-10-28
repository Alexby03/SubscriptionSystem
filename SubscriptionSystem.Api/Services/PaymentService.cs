using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Data;
using SubscriptionSystem.Enums;
using SubscriptionSystem.Entities;
using SubscriptionSystem.Results;

namespace SubscriptionSystem.Services;

public class PaymentService
{
    private readonly AppDbContext _db;

    public PaymentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Payment?> CreatePaymentAsync(Guid invoiceId, Guid customerId, decimal amount)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.CustomerId == customerId);
        if (invoice == null || invoice.Status == InvoiceStatus.Paid)
            return null;
        var payment = new Payment(amount, customerId, invoiceId);
        await _db.Payments.AddAsync(payment);
        await _db.SaveChangesAsync();
        return payment;
    }

    public async Task<ProcessPaymentResult> ProcessPaymentAsync(Guid paymentId)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        if (payment == null)
            return ProcessPaymentResult.NotFound;
        if (payment.Status == PaymentStatus.Paid)
            return ProcessPaymentResult.AlreadyProcessed;
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == payment.InvoiceId);
        if (invoice != null)
        {
            payment.ProcessPayment();
            invoice.MarkPaid();
            await _db.SaveChangesAsync();
            return ProcessPaymentResult.Success;
        }
        return ProcessPaymentResult.Declined;
    }

    /*public async Task<ProcessPaymentResult> RefundPaymentAsync(Guid paymentId)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        if (payment == null)
            return ProcessPaymentResult.NotFound;
        else if (payment.Status == PaymentStatus.Refunded)
            return ProcessPaymentResult.Refunded;
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == payment.InvoiceId);
        if (invoice != null)
        {
            payment.Refund();
            invoice.Status = InvoiceStatus.Refunded;
            await _db.SaveChangesAsync();
            return ProcessPaymentResult.Success;
        }
        return ProcessPaymentResult.Failed;
    }*/

    public async Task<List<Payment>> GetPaymentsForCustomerAsync(Guid customerId)
    {
        return await _db.Payments.Where(p => p.CustomerId == customerId).ToListAsync();
    }
}