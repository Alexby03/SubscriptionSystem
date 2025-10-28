namespace SubscriptionSystem.Entities;

using SubscriptionSystem.Configuration;
using SubscriptionSystem.Enums;
using SubscriptionSystem.Results;

public class Subscription
{
    public Guid SubscriptionId { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public int PlanId { get; private set; }
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;
    public DateTime StartDate { get; private set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; private set; }
    public BillingCycle BillingCycle { get; private set; } = BillingCycle.Monthly; // default monthly
    public DateTime NextBillingDate { get; private set; }

    private Subscription() { } // EF / ORM support

    public Subscription(int planId, Guid customerId, int billingCycle, DateTime? startDate = null)
    {
        PlanId = planId;
        CustomerId = customerId;
        StartDate = startDate ?? DateTime.UtcNow;
        BillingCycle = (BillingCycle)billingCycle;
        Status = planId == 0 ? SubscriptionStatus.Trial : SubscriptionStatus.Active;

        // Initial next billing date
        NextBillingDate = BillingCycle switch
        {
            BillingCycle.Monthly => StartDate.AddSeconds(TimeConstants.MONTH),
            BillingCycle.Yearly => StartDate.AddSeconds(TimeConstants.MONTH * 12),
            _ => StartDate.AddSeconds(TimeConstants.MONTH)
        };
        EndDate = BillingCycle switch
        {
            BillingCycle.Monthly => EndDate = StartDate.AddSeconds(TimeConstants.MONTH),
            BillingCycle.Yearly => EndDate = StartDate.AddSeconds(TimeConstants.MONTH * 12),
            _ => EndDate = StartDate.AddSeconds(TimeConstants.MONTH)
        };
    }

    // Activates subscription if currently in Trial or other non-active state
    public void Activate()
    {
        if (Status != SubscriptionStatus.Active)
            Status = SubscriptionStatus.Active;
    }

    // Cancels the subscription at a given end date
    public GenericResult Cancel(DateTime? endDate = null)
    {
        if (Status == SubscriptionStatus.Canceled)
            return GenericResult.Failed;

        Status = SubscriptionStatus.Canceled;
        EndDate = endDate ?? DateTime.UtcNow;

        return GenericResult.Success;
    }

    // Changes the plan and optionally the billing cycle
    public GenericResult ChangePlan(int newPlanId, BillingCycle? newCycle = null)
    {
        PlanId = newPlanId;
        if (newCycle.HasValue)
        {
            BillingCycle = newCycle.Value;
            return GenericResult.Success;
        }
        // Reset next billing date for new plan
        NextBillingDate = DateTime.UtcNow; 
        return GenericResult.Success;
    }

    // Moves NextBillingDate forward according to BillingCycle
    public GenericResult AdvanceBillingDate()
    {
        NextBillingDate = BillingCycle switch
        {
            BillingCycle.Monthly => NextBillingDate.AddSeconds(TimeConstants.MONTH),
            BillingCycle.Yearly => NextBillingDate.AddSeconds(TimeConstants.MONTH * 12),
            _ => NextBillingDate.AddSeconds(TimeConstants.MONTH)
        };
        if(EndDate != null)
        {
            EndDate = BillingCycle switch
            {
                BillingCycle.Monthly => EndDate.Value.AddSeconds(TimeConstants.MONTH),
                BillingCycle.Yearly => EndDate.Value.AddSeconds(TimeConstants.MONTH*12),
                _ => NextBillingDate.AddSeconds(TimeConstants.MONTH)
            };
        }

        return GenericResult.Success;
    }

    // Checks if subscription is active and not expired
    public bool IsActive()
    {
        return Status == SubscriptionStatus.Active && (EndDate == null || EndDate > DateTime.UtcNow);
    }
}
