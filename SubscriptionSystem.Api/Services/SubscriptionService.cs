namespace SubscriptionSystem.Services;

using SubscriptionSystem.Data;
using SubscriptionSystem.Models;
using SubscriptionSystem.Results;
using Microsoft.EntityFrameworkCore;

public class SubscriptionService
{
    private readonly AppDbContext _db;

    public SubscriptionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Subscription> CreateSubscriptionAsync(Guid customerId, int planId, DateTime endDate)
    {
        var subscription = new Subscription(planId, customerId, endDate);
        await _db.Subscriptions.AddAsync(subscription);

        var invoice = new Invoice(
            subscription.StartDate.AddDays(30),
            Plans.All[planId].Price,
            customerId,
            subscription.SubscriptionId
        );
        await _db.Invoices.AddAsync(invoice);

        await _db.SaveChangesAsync();

        Console.WriteLine($"Invoice generated for CustomerId={customerId}, Amount={invoice.Amount:C}, DueDate={invoice.DueDate}");

        return subscription;
    }

    public async Task<RenewSubscriptionResult> RenewSubscriptionAsync(Guid subscriptionId)
    {
        var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
        if (subscription == null)
            return RenewSubscriptionResult.Failed;
        var result = subscription.Renew();
        await _db.SaveChangesAsync();
        return result;
    }

    public async Task<GenericResult> CancelSubscriptionAsync(Guid subscriptionId)
    {
        var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
        if (subscription == null)
            return GenericResult.Failed;
        var result = subscription.Cancel();
        await _db.SaveChangesAsync();
        return GenericResult.Success;
    }

    public async Task<GenericResult> UpgradePlanAsync(Guid subscriptionId, int newPlanId)
    {
        var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
        if (subscription == null)
            return GenericResult.Failed;
        var result = subscription.UpgradePlan(newPlanId);
        await _db.SaveChangesAsync();
        return result;
    }
}
