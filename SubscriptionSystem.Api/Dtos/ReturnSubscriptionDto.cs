namespace SubscriptionSystem.Dtos;
public record SubscriptionDto(
    Guid SubscriptionId,
    Guid CustomerId,
    string Status,
    int PlanId,
    DateTime StartDate,
    DateTime EndDate
);