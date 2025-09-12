namespace SubscriptionSystem.Services;

using SubscriptionSystem.Models;

public class SubscriptionService
{
    private readonly List<Subscription> _subscriptions;
    private readonly List<Invoice> _invoices;

    public SubscriptionService(List<Subscription> subscriptions, List<Invoice> invoices)
    {
        _subscriptions = subscriptions;
        _invoices = invoices;
    }

    public Subscription CreateSubscription(Customer customer, int planId, DateTime endDate)
    {
        var subscription = new Subscription(planId, customer.CustomerId, endDate);
        _subscriptions.Add(subscription);
        GenerateInvoice(subscription);
        return subscription;
    }

    private void GenerateInvoice(Subscription subscription)
    {
        var invoice = new Invoice(subscription.StartDate.AddDays(30), GetPlanPrice(subscription.PlanId), subscription.CustomerId, subscription.SubscriptionId);
        _invoices.Add(invoice);
    }

    private decimal GetPlanPrice(int planId)
    {
        return Plans.All[planId].Price;
    }

    public IEnumerable<Subscription> GetSubscriptions(Guid customerId)
    {
        return _subscriptions.Where(s => s.CustomerId == customerId);
    }

    public IEnumerable<Invoice> GetInvoices(Guid customerId)
    {
        return _invoices.Where(i => i.CustomerId == customerId);
    }
        
}
