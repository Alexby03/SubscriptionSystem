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

    public Subscription(int planId, Guid customerId, DateTime endDate)
    {
        PlanId = planId;
        EndDate = endDate;
        CustomerId = customerId;
        Status = SubscriptionStatus.Active;
    }

    public RenewSubscriptionResult Renew()
    {
        if (Status == SubscriptionStatus.Trial)
        {
            return RenewSubscriptionResult.Trial;
        }
        EndDate = EndDate.AddDays(30);
        Status = SubscriptionStatus.Active;
        return RenewSubscriptionResult.Success;
    }

    public GenericResult Cancel()
    {
        Status = SubscriptionStatus.Canceled;
        return GenericResult.Success; //TODO
    }

    public bool UpgradePlan()
    {
        return true; //TODO
    }

}