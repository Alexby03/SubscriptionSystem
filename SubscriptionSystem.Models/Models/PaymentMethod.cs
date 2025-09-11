namespace SubscriptionSystem.Models;

public class PaymentMethod
{
    public Guid PaymentMethodId { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public string Type { get; private set; }
    public string LastFourDigits { get; private set; }
    public DateTime Expiry { get; private set; }
    public string GatewayToken { get; private set; } //token from payment gateway

    public PaymentMethod(string type, string lastFourDigits, DateTime expiry, string gatewayToken, Guid customerId)
    {
        PaymentMethodId = Guid.NewGuid();
        Type = type;
        LastFourDigits = lastFourDigits;
        Expiry = expiry;
        GatewayToken = gatewayToken;
        CustomerId = customerId;
    }

    public bool Validate()
    {
        return true; //TODO
    }

}