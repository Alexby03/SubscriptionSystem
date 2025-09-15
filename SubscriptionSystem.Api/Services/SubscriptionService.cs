namespace SubscriptionSystem.Services;

using SubscriptionSystem.Data;
using SubscriptionSystem.Entities;
using SubscriptionSystem.Results;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Events;
using SubscriptionSystem.Outbox;
using System.Text.Json;

public class SubscriptionService
{
    private readonly AppDbContext _db;

    public SubscriptionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Subscription> CreateSubscriptionAsync(Guid customerId, int planId, DateTime endDate)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var subscription = new Subscription(planId, customerId, endDate);
            _db.Subscriptions.Add(subscription);
            var invoice = new Invoice(
                subscription.StartDate.AddDays(30),
                Plans.All[planId].Price,
                customerId,
                subscription.SubscriptionId
            );
            _db.Invoices.Add(invoice);
            var @event = new SubscriptionCreatedEvent(customerId, planId);
            var outboxEvent = new OutboxEvent()
            {
                EventType = nameof(SubscriptionCreatedEvent),
                Payload = JsonSerializer.Serialize(@event)
            };
            _db.OutboxEvents.Add(outboxEvent);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            Console.WriteLine($"Invoice generated for CustomerId={customerId}, Amount={invoice.Amount:C}, DueDate={invoice.DueDate}");
            return subscription;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<RenewSubscriptionResult> RenewSubscriptionAsync(Guid subscriptionId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
            if (subscription == null)
                return RenewSubscriptionResult.Failed;
            var result = subscription.Renew();
            if (result == RenewSubscriptionResult.Trial)
                return RenewSubscriptionResult.Trial;
            if (result == RenewSubscriptionResult.Canceled)
                return RenewSubscriptionResult.Canceled;
            var invoice = new Invoice(
                subscription.EndDate.AddDays(30),
                Plans.All[subscription.PlanId].Price,
                subscription.CustomerId,
                subscription.SubscriptionId
            );
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            Console.WriteLine($"Invoice generated for CustomerId={subscription.CustomerId}, Amount={invoice.Amount:C}, DueDate={invoice.DueDate}");
            return RenewSubscriptionResult.Success;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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
