namespace SubscriptionSystem.Billing;

public static class ProrationCalculator
{
    public static decimal CalculateProratedAmount(
        decimal oldPrice,
        decimal newPrice,
        DateTime periodStart,
        DateTime periodEnd,
        DateTime changeDate)
    {
        var totalSeconds = (periodEnd - periodStart).TotalSeconds;
        var remainingSeconds = (periodEnd - changeDate).TotalSeconds;

        if (totalSeconds <= 0 || remainingSeconds <= 0)
            return 0;

        var fractionRemaining = remainingSeconds / totalSeconds;

        var oldPortion = oldPrice * (decimal)fractionRemaining;
        var newPortion = newPrice * (decimal)fractionRemaining;

        var difference = newPortion - oldPortion;

        return Math.Round(difference, 2);
    }
}
