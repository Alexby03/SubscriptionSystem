using SubscriptionSystem.Entities;
using SubscriptionSystem.Dtos;
using SubscriptionSystem.Results;
using SubscriptionSystem.Services;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Data;
using Microsoft.AspNetCore.Mvc;

public static class SubscriptionEndpoint
{
    public static void MapSubscriptionEndpoints(this WebApplication app)
    {
        //get all subscriptions for a customer
        app.MapGet("/customers/{id}/subscriptions", async (Guid id, AppDbContext db) =>
        {
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} was not found.");
            var customerSubscriptions = await db.Subscriptions.Where(s => s.CustomerId == id).ToListAsync();
            return Results.Ok(customerSubscriptions);
        });

        //create a subscription for a customer
        app.MapPost("/customers/{id}/subscriptions", async (Guid id, CreateSubscriptionDto dto, AppDbContext db, [FromServices] SubscriptionService service) =>
        {
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} was not found.");
            var newSubscription = await service.CreateSubscriptionAsync(
                customer.CustomerId,
                dto.PlanId,
                DateTime.UtcNow.AddDays(30)
            );
            return Results.Created($"/customers/{id}/subscriptions/{newSubscription.SubscriptionId}", newSubscription);
        });

        //cancel a subscription for a customer
        app.MapPut("/customers/{customerId}/subscriptions/{subscriptionId}/cancel", async (Guid customerId, Guid subscriptionId, AppDbContext db, [FromServices] SubscriptionService service) =>
        {
            var subscription = await db.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId && s.CustomerId == customerId);
            if (subscription == null)
                return Results.NotFound($"Subscription with id {subscriptionId} for customer {customerId} was not found.");
            var result = await service.CancelSubscriptionAsync(subscriptionId);
            if (result == GenericResult.Failed)
                return Results.BadRequest($"Subscription with id {subscriptionId} could not be cancelled.");
            await db.SaveChangesAsync();
            return Results.Ok($"Subscription with id {subscriptionId} cancelled successfully.");
        });

        //renew a subscription for a customer
        app.MapPut("/customers/{customerId}/subscriptions/{subscriptionId}/renew", async (Guid customerId, Guid subscriptionId, AppDbContext db, [FromServices] SubscriptionService service) =>
        {
            var subscription = await db.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId && s.CustomerId == customerId);
            if (subscription == null)
                return Results.NotFound($"Subscription with id {subscriptionId} for customer {customerId} was not found.");
            var renewResult = service.RenewSubscriptionAsync(subscriptionId).Result;
            if (renewResult == RenewSubscriptionResult.Failed)
                return Results.BadRequest($"Subscription with id {subscriptionId} could not be renewed.");
            if (renewResult == RenewSubscriptionResult.Trial)
                return Results.BadRequest($"Subscription with id {subscriptionId} is in trial period and cannot be renewed.");
            if (renewResult == RenewSubscriptionResult.Canceled)
                return Results.BadRequest($"Subscription with id {subscriptionId} is canceled and cannot be renewed.");
            await db.SaveChangesAsync();
            return Results.Ok($"Subscription with id {subscriptionId} renewed successfully until {subscription.EndDate}.");
        });
    }
}