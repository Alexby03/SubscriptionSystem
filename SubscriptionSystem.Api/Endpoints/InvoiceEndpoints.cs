using SubscriptionSystem.Models;
using SubscriptionSystem.Results;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app, List<Customer> customers, List<Invoice> invoices)
    {
        //list invoices for a customer
        app.MapGet("/customers/{id}/invoices", (Guid id) =>
        {
            var customerInvoices = invoices.Where(i => i.CustomerId == id).ToList();
        });

        //details of one invoice
        app.MapGet("/invoices/{id}", (Guid id) =>
        {
            var invoice = invoices.FirstOrDefault(i => i.InvoiceId == id);
            return invoice is not null ? Results.Ok(invoice) : Results.NotFound();
        });

        //mark an invoice as paid (manual or bank signal)
        app.MapPost("/invoices/{id}/markPaid", (Guid id) =>
        {
            var invoice = invoices.FirstOrDefault(i => i.InvoiceId == id);
            if (invoice is null)
            {
                return Results.NotFound();
            }
            var result = invoice.MarkPaid();
            return result is PayInvoiceResult.Success ? Results.Ok(invoice) : Results.BadRequest();
        });
    }
}