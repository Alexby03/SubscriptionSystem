using SubscriptionSystem.Services;
using SubscriptionSystem.Results;
using SubscriptionSystem.Data;
using Microsoft.AspNetCore.Mvc;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this WebApplication app)
    {
        //create payment for invoice
        app.MapPost("/customers/{customerId}/invoices/{invoiceId}/payments", async (Guid customerId, Guid invoiceId, [FromServices] PaymentService service, AppDbContext db) =>
        {
            var invoice = await db.Invoices.FindAsync(invoiceId);
            if (invoice == null)
                return Results.NotFound("Invoice not found.");

            var payment = await service.CreatePaymentAsync(invoiceId, customerId, invoice.Amount);
            return payment is null
                ? Results.BadRequest("Payment could not be created (invoice already paid or invalid).")
                : Results.Created($"/payments/{payment.PaymentId}", payment);
        });

        //process payment
        app.MapPost("/payments/{paymentId}/process", async (Guid paymentId, [FromServices] PaymentService service) =>
        {
            var result = await service.ProcessPaymentAsync(paymentId);
            return result switch
            {
                ProcessPaymentResult.Success => Results.Ok("Payment processed successfully."),
                ProcessPaymentResult.NotFound => Results.NotFound("Payment not found."),
                _ => Results.BadRequest("Payment could not be processed.")
            };
        });

        //refund payment
        app.MapPost("/payments/{paymentId}/refund", async (Guid paymentId, [FromServices] PaymentService service) =>
        {
            var result = await service.RefundPaymentAsync(paymentId);
            return result switch
            {
                ProcessPaymentResult.Refunded => Results.Ok("Payment already refunded."),
                ProcessPaymentResult.NotFound => Results.NotFound("Payment not found."),
                ProcessPaymentResult.Success => Results.Ok("Payment refunded successfully."),
                _ => Results.BadRequest("Refund failed.")
            };
        });

        //list all payments for customer
        app.MapGet("/customers/{customerId}/payments", async (Guid customerId, [FromServices] PaymentService service) =>
        {
            var payments = await service.GetPaymentsForCustomerAsync(customerId);
            return Results.Ok(payments);
        });
    }
}