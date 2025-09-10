namespace SubscriptionSystem.Models;
using SubscriptionSystem.Enums;
public class Subscription
{
    public Guid SubscriptionId { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public Plan Plan { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime TrialEndDate { get; private set; }

    public Subscription(Plan plan, DateTime startDate, DateTime endDate, DateTime trialEndDate, Guid customerId)
    {
        Status = SubscriptionStatus.Active;
        Plan = plan;
        StartDate = startDate;
        EndDate = endDate;
        TrialEndDate = trialEndDate;
        CustomerId = customerId;
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