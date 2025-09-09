namespace SubscriptionSystem.Models;

public class PaymentMethod
{
    public Guid GuId { get; private set; } = Guid.NewGuid();
    public string Type { get; private set; }
    public string LastFourDigits { get; private set; }
    public DateTime Expiry { get; private set; }

    public PaymentMethod(string type, string lastFourDigits, DateTime expiry)
    {
        GuId = Guid.NewGuid();
        Type = type;
        LastFourDigits = lastFourDigits;
        Expiry = expiry;
    }

    public bool Validate(DateTime expiry)
    {
        return true; //TODO
    }

}