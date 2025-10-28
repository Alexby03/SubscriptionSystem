namespace SubscriptionSystem.Services;

using SubscriptionSystem.Data;
using SubscriptionSystem.Entities;
using SubscriptionSystem.Enums;
using SubscriptionSystem.Results;
using SubscriptionSystem.Events;
using SubscriptionSystem.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SubscriptionSystem.Billing;
using SubscriptionSystem.Configuration;

public class SubscriptionService
{
    private readonly AppDbContext _db;

    public SubscriptionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Subscription> CreateSubscriptionAsync(
        Guid customerId,
        int planId,
        int billingCycle,
        DateTime? startDate = null)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var subscription = new Subscription(planId, customerId, billingCycle, startDate);

            _db.Subscriptions.Add(subscription);

            var serviceStart = subscription.StartDate;
            var serviceEnd = subscription.NextBillingDate;
            var dueDate = serviceEnd;

            var invoice = new Invoice(
                customerId: customerId,
                amount: Plans.All[planId].Price,
                dueDate: dueDate,
                subscriptionId: subscription.SubscriptionId,
                servicePeriodStart: serviceStart,
                servicePeriodEnd: serviceEnd
            );

            _db.Invoices.Add(invoice);

            // subscription created event
            var @event = new SubscriptionCreatedEvent(customerId, planId);
            var outboxEvent = new OutboxEvent
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

    public async Task<RenewSubscriptionResult> AdvanceSubscriptionBillingAsync(Guid subscriptionId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
            if (subscription == null)
                return RenewSubscriptionResult.Failed;

            var result = subscription.AdvanceBillingDate();
            if (result == GenericResult.Failed)
                return RenewSubscriptionResult.Failed;

            var serviceStart = subscription.NextBillingDate;
            var serviceEnd = subscription.BillingCycle switch
            {
                BillingCycle.Monthly => serviceStart.AddSeconds(TimeConstants.MONTH),
                BillingCycle.Yearly => serviceStart.AddSeconds(TimeConstants.MONTH*12),
                _ => serviceStart.AddSeconds(TimeConstants.MONTH)
            };
            var dueDate = serviceEnd;

            var invoice = new Invoice(
                customerId: subscription.CustomerId,
                amount: Plans.All[subscription.PlanId].Price,
                dueDate: dueDate,
                subscriptionId: subscription.SubscriptionId,
                servicePeriodStart: serviceStart,
                servicePeriodEnd: serviceEnd
            );

            var @event = new SubscriptionBillingAdvancedEvent(subscription.SubscriptionId, subscription.NextBillingDate);
            _db.OutboxEvents.Add(new OutboxEvent
            {
                EventType = nameof(SubscriptionBillingAdvancedEvent),
                Payload = JsonSerializer.Serialize(@event)
            });

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine($"Invoice generated for SubscriptionId={subscription.SubscriptionId}, CustomerId={subscription.CustomerId}, Amount={invoice.Amount:C}");
            return RenewSubscriptionResult.Success;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<GenericResult> CancelSubscriptionAsync(Guid subscriptionId, DateTime? endDate = null)
    {
        var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
        if (subscription == null)
            return GenericResult.Failed;

        var result = subscription.Cancel(endDate);
        await _db.SaveChangesAsync();
        return result;
    }

    public async Task<GenericResult> UpgradePlanAsync(Guid subscriptionId, int newPlanId)
    {
        var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
        if (subscription == null)
            return GenericResult.Failed;

        var oldPlan = Plans.All[subscription.PlanId];
        var newPlan = Plans.All[newPlanId];

        var now = DateTime.UtcNow;
        var periodStart = subscription.NextBillingDate.AddSeconds(-1*TimeConstants.MONTH);
        var periodEnd = subscription.NextBillingDate;

        var proratedAmount = ProrationCalculator.CalculateProratedAmount(
            oldPlan.Price,
            newPlan.Price,
            periodStart,
            periodEnd,
            now
        );

        if (proratedAmount != 0)
        {
            var invoice = new Invoice(
                customerId: subscription.CustomerId,
                amount: Math.Abs(proratedAmount), // dont check for credit note
                dueDate: now.AddSeconds(TimeConstants.WEEK), // flexible
                subscriptionId: subscription.SubscriptionId,
                servicePeriodStart: now,
                servicePeriodEnd: subscription.NextBillingDate
            );

            if (proratedAmount < 0)
            {
                // no credit note, for now just mark paid.
                invoice.MarkPaid();
            }

            _db.Invoices.Add(invoice);
        }

        var upgradeEvent = new SubscriptionUpgradedEvent(
            subscription.SubscriptionId,
            oldPlan.PlanId,
            newPlanId,
            proratedAmount
        );
        _db.OutboxEvents.Add(new OutboxEvent
        {
            EventType = nameof(SubscriptionUpgradedEvent),
            Payload = JsonSerializer.Serialize(upgradeEvent)
        });

        subscription.ChangePlan(newPlanId);

        await _db.SaveChangesAsync();
        return GenericResult.Success;
    }

}
