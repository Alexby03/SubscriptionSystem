using SubscriptionSystem.Models;
using SubscriptionSystem.Results;

namespace SubscriptionSystem.Models;

public class Customer
{
    public Guid GuId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string BillingAddress { get; private set; }

    private readonly List<PaymentMethod> _paymentMethods = new();
    public IReadOnlyList<PaymentMethod> PaymentMethods
    {
        get { return _paymentMethods.AsReadOnly(); }
    }

    private readonly List<Subscription> _subscriptions = new();

    public Customer(string name, string email, string billingAddress)
    {
        Name = name;
        Email = email;
        BillingAddress = billingAddress;
    }

    public AddPaymentMethodResult AddPaymentMethod(PaymentMethod paymentMethod)
    {
        _paymentMethods.Add(paymentMethod);
        return AddPaymentMethodResult.Success; //TODO
    }

    public SubscribeResult Subscribe(Plan plan)
    {
        return SubscribeResult.Success; //TODO
    }
    
    public UnsubscribeResult CancelSubscription(Plan plan)
    {
        return UnsubscribeResult.Success; //TODO
    }

}