namespace SubscriptionSystem.Dtos;

public record AddPaymentMethodDto(string Type, string LastFourDigits, DateTime Expiry);