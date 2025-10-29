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
        var elapsedSeconds = (changeDate - periodStart).TotalSeconds;
        var remainingSeconds = totalSeconds - elapsedSeconds;

        if (totalSeconds <= 0 || remainingSeconds <= 0)
            return 0;

        var fractionRemaining = (decimal)remainingSeconds / (decimal)totalSeconds;

        var difference = (newPrice - oldPrice) * fractionRemaining;

        return Math.Round(difference, 2);

    }
}
