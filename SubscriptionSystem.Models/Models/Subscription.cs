namespace SubscriptionSystem.Models;
using SubscriptionSystem.Enums;
public class Subscription
{
    public Guid SubscriptionId { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;
    public int PlanId { get; private set; }
    public DateTime StartDate { get; private set; } = DateTime.UtcNow;
    public DateTime EndDate { get; private set; }
    public DateTime? TrialEndDate { get; private set; }

    public Subscription(int planId, Guid customerId, DateTime endDate, DateTime? trialEndDate = null)
    {
        PlanId = planId;
        EndDate = endDate;
        CustomerId = customerId;
        TrialEndDate = trialEndDate;
    }

    public bool Renew()
    {
        return true; //TODO
    }

    public bool Cancel()
    {
        return true; //TODO
    }

    public bool UpgradePlan()
    {
        return true; //TODO
    }

}