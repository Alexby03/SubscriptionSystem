using SubscriptionSystem.Models;
using SubscriptionSystem.Dtos;
using SubscriptionSystem.Results;

public static class SubscriptionEndpoint
{
    public static void MapSubscriptionEndpoints(this WebApplication app, List<Customer> customers, List<Subscription> subscriptions, List<Invoice> invoices) 
    {
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
            var plan = Plans.All[dto.PlanId];
            var newSubscription = new Subscription(dto.PlanId, id, dto.EndDate, dto.TrialEndDate);
            subscriptions.Add(newSubscription);
            return Results.Created($"Subscription with id {newSubscription.SubscriptionId} created for customer {id}.", newSubscription);
        });
    }
}