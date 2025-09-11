namespace SubscriptionSystem.Models;

using SubscriptionSystem.Enums;
public static class Plans
{
    public static readonly Dictionary<int, Plan> All = new Dictionary<int, Plan>
    {
        { 0, new Plan
            (
                0,
                "Trial",
                0.00m,
                BillingCycle.Monthly,
                new List<string> { "Base feature 1", "Base feature 2" }
            )
        },
        { 1, new Plan
            (
                1,
                "Basic",
                59.99m,
                BillingCycle.Monthly,
                new List<string> { "Base feature 1", "Base feature 2", "Basic Feature 1" }
            )
        },
        { 2, new Plan
            (
                2,
                "Business",
                159.99m,
                BillingCycle.Monthly,
                new List<string> { "Base feature 1", "Base feature 2", "Premium Feature 1" }
            )
        },
        { 3, new Plan
            (
                3,
                "Enterprise",
                299.99m,
                BillingCycle.Monthly,
                new List<string> { "Base feature 1", "Premium feature 1", "Enterprise Feature 1" }
            )
        }
    };
}