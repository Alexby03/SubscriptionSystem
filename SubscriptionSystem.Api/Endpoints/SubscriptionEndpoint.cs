using SubscriptionSystem.Models;
using SubscriptionSystem.Dtos;
using SubscriptionSystem.Results;
using SubscriptionSystem.Services;

public static class SubscriptionEndpoint
{
    public static void MapSubscriptionEndpoints(this WebApplication app, List<Customer> customers, List<Subscription> subscriptions, List<Invoice> invoices)
    {
        var subscriptionService = new SubscriptionService(subscriptions, invoices);

        //get all subscriptions for a customer
        app.MapGet("/customers/{id}/subscriptions", (Guid id) =>
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == id);
            if (customers == null)
                return Results.NotFound($"Customer with id {id} was not found.");
            var customerSubscriptions = subscriptions.Where(s => s.CustomerId == id).ToList();
            return Results.Ok(customerSubscriptions);
        });

        //create a subscription for a customer
        app.MapPost("/customers/{id}/subscriptions", (Guid id, CreateSubscriptionDto dto) =>
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} was not found.");

            var newSubscription = subscriptionService.CreateSubscription(customer, dto.PlanId, DateTime.UtcNow.AddDays(30));
            subscriptions.Add(newSubscription);
            return Results.Created($"Subscription with id {newSubscription.SubscriptionId} created for customer {id}.", newSubscription);
        });

        //cancel a subscription for a customer
        app.MapPut("/customers/{customerId}/subscriptions/{subcriptionId}/cancel", (Guid customerId, Guid subcriptionId) =>
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
                return Results.NotFound($"Customer with id {customerId} was not found.");
            var subscription = subscriptions.FirstOrDefault(s => s.SubscriptionId == subcriptionId && s.CustomerId == customerId);
            if (subscription == null)
                return Results.NotFound($"Subscription with id {subcriptionId} for customer {customerId} was not found.");
            var genericResult = subscription.Cancel();
            if (genericResult == GenericResult.Failed)
                return Results.BadRequest($"Subscription with id {subcriptionId} could not be cancelled.");
            return Results.Ok($"Subscription with id {subcriptionId} cancelled successfully.");
        });

        //renew a subscription for a customer
        app.MapPut("/customers/{customerId}/subscriptions/{subcriptionId}/renew", (Guid customerId, Guid subcriptionId) =>
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
                return Results.NotFound($"Customer with id {customerId} was not found.");
            var subscription = subscriptions.FirstOrDefault(s => s.SubscriptionId == subcriptionId && s.CustomerId == customerId);
            if (subscription == null)
                return Results.NotFound($"Subscription with id {subcriptionId} for customer {customerId} was not found.");
            var renewResult = subscription.Renew();
            if (renewResult == RenewSubscriptionResult.Failed)
                return Results.BadRequest($"Subscription with id {subcriptionId} could not be renewed.");
            if (renewResult == RenewSubscriptionResult.Trial)
                return Results.BadRequest($"Subscription with id {subcriptionId} is in trial period and cannot be renewed.");
            return Results.Ok($"Subscription with id {subcriptionId} renewed successfully until {subscription.EndDate}.");
        });
    }
}