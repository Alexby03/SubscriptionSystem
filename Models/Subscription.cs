namespace SubscriptionSystem.Models;
using SubscriptionSystem.Enums;
public class Subscription
{
    public Guid UuId { get; private set; } = Guid.NewGuid();
    public SubscriptionStatus SubscriptionStatus { get; private set; }
    public Plan Plan { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime TrialEndDate { get; private set; }

    public Subscription(Plan plan, DateTime startDate, DateTime endDate, DateTime trialEndDate) {
        SubscriptionStatus = SubscriptionStatus.Active;
        Plan = plan;
        StartDate = startDate;
        EndDate = endDate;
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