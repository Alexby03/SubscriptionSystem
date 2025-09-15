namespace SubscriptionSystem.Entities;

using SubscriptionSystem.Enums;
public class Plan
{
    public int PlanId { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public BillingCycle BillingCycle { get; private set; }
    public string Features { get; private set; }

    public Plan(int planId, string name, decimal price, BillingCycle billingCycle, string features)
    {
        PlanId = planId;
        Name = name;
        Price = price;
        BillingCycle = billingCycle;
        Features = features;
    }
    
}