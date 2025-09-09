namespace SubscriptionSystem.Models;

using SubscriptionSystem.Enums;
public static class PlanFactory
{
    public static readonly Plan Trial = new Plan
    (
        "Trial",
        0.00m,
        BillingCycle.Monthly,
        new List<string> { "Base feature 1", "Base feature 2" }
    );

    public static readonly Plan Basic = new Plan
    (
        "Basic",
        59.99m,
        BillingCycle.Monthly,
        new List<string> { "Base feature 1", "Base feature 2", "Basic Feature 1" }
    );

    public static readonly Plan Business = new Plan
    (
        "Business",
        159.99m,
        BillingCycle.Monthly,
        new List<string> { "Base feature 1", "Base feature 2", "Premium Feature 1" }
    );

    public static readonly Plan Enterprise = new Plan
    (
        "Enterprise",
        299.99m,
        BillingCycle.Monthly,
        new List<string> { "Base feature 1", "Premium feature 1", "Enterprise Feature 1" }
    );

    public static IEnumerable<Plan> GetAllPlans()
    {
        return new List<Plan> { Trial, Basic, Business, Enterprise };
    }

}