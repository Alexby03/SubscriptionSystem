namespace SubscriptionSystem.Models;

using SubscriptionSystem.Enums;
public class Plan
{
    public Guid UuId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public BillingCycle BillingCycle { get; private set; }

    private List<string> _features = new List<string> { };
    public IReadOnlyList<string> Features
    {
        get { return _features.AsReadOnly(); }
    }

    public Plan(string name, decimal price, BillingCycle billingCycle, IEnumerable<string> features)
    {
        Name = name;
        Price = price;
        BillingCycle = billingCycle;
        _features.AddRange(features);
    }
    
}