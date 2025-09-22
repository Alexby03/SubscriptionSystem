using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Data;
using SubscriptionSystem.Entities;
using SubscriptionSystem.Results;
using SubscriptionSystem.Services;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app)
    {
        //list invoices for a customer
        app.MapGet("/customers/{id}/invoices", async (Guid id, InvoiceService service) =>
        {
            var customerInvoices = await service.GetInvoicesForCustomerAsync(id);
            if (customerInvoices.Count == 0)
                return Results.NotFound($"No invoices found for customer with id {id}.");
            return Results.Ok(customerInvoices);
        });

        //details of one invoice
        app.MapGet("/invoices/{id}", async (Guid id, [FromServices] InvoiceService service) =>
        {
            var invoice = await service.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return Results.NotFound($"No invoice found for customer with id {id}.");
            return Results.Ok(invoice);
        });

        // mark invoice as paid
        app.MapPost("/invoices/{id}/markPaid", async (Guid id, [FromServices] InvoiceService service) =>
        {
            var result = await service.MarkPaidAsync(id);
            return result switch
            {
                PayInvoiceResult.Success => Results.Ok(),
                PayInvoiceResult.NotFound => Results.NotFound(),
                _ => Results.BadRequest()
            };
        });

        // list unpaid invoices
        app.MapGet("/customers/{id}/invoices/unpaid", async (Guid id, [FromServices] InvoiceService service) =>
        {
            var unpaidInvoices = await service.GetUnpaidInvoicesForCustomerAsync(id);
            return Results.Ok(unpaidInvoices);
        });
    }
}