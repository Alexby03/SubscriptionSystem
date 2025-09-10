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
    public IReadOnlyList<Subscription> Subscriptions
    {
        get { return _subscriptions.AsReadOnly(); }
    }

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

    public AddPaymentMethodResult AddPaymentMethod(PaymentMethod paymentMethod)
    {
        _paymentMethods.Add(paymentMethod);
        return AddPaymentMethodResult.Success;
    }

    public GenericResult RemovePaymentMethod(Guid paymentMethodId)
    {
        var paymentMethod = _paymentMethods.FirstOrDefault(pm => pm.PaymentMethodId == paymentMethodId);
        if (paymentMethod == null)
            return GenericResult.Failed;

        _paymentMethods.Remove(paymentMethod);
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

}