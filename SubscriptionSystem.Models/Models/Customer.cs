using SubscriptionSystem.Enums;
using SubscriptionSystem.Models;
using SubscriptionSystem.Results;

namespace SubscriptionSystem.Models;

public class Customer
{
    public Guid CustomerId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string BillingAddress { get; private set; }
    public CustomerStatus Status { get; private set; } = CustomerStatus.Active;

    public Customer(string name, string email, string billingAddress)
    {
        Name = name;
        Email = email;
        BillingAddress = billingAddress;
    }

    public GenericResult Update(string name, string email, string billingAddress)
    {
        Name = name;
        Email = email;
        BillingAddress = billingAddress;
        return GenericResult.Success;
    }

    public SubscribeResult Subscribe(Plan plan)
    {
        return SubscribeResult.Success; //TODO
    }

    public UnsubscribeResult CancelSubscription(Plan plan)
    {
        return UnsubscribeResult.Success; //TODO
    }
    
    public GenericResult MarkInactive()
    {
        Status = CustomerStatus.Inactive;
        return GenericResult.Success;
    }

}