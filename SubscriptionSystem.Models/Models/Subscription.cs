namespace SubscriptionSystem.Models;

using SubscriptionSystem.Enums;
using SubscriptionSystem.Results;

public class Subscription
{
    public Guid SubscriptionId { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;
    public int PlanId { get; private set; }
    public DateTime StartDate { get; private set; } = DateTime.UtcNow;
    public DateTime EndDate { get; private set; }
    private Subscription() { }

    public Subscription(int planId, Guid customerId, DateTime endDate)
    {
        PlanId = planId;
        CustomerId = customerId;
        EndDate = endDate;
    }

    public RenewSubscriptionResult Renew()
    {
        if (Status == SubscriptionStatus.Trial)
            return RenewSubscriptionResult.Trial;

        EndDate = EndDate.AddDays(30);
        Status = SubscriptionStatus.Active;
        return RenewSubscriptionResult.Success;
    }

    public GenericResult Cancel()
    {
        if (Status == SubscriptionStatus.Canceled)
            return GenericResult.Failed;

        Status = SubscriptionStatus.Canceled;
        return GenericResult.Success;
    }

    public GenericResult UpgradePlan(int newPlanId)
    {
        PlanId = newPlanId;
        EndDate = DateTime.UtcNow.AddDays(30);
        return GenericResult.Success;
    }
}
