using SubscriptionSystem.Enums;

namespace SubscriptionSystem.Dtos;

public record CreateSubscriptionDto(Guid CustomerId, int PlanId, DateTime EndDate, DateTime TrialEndDate);