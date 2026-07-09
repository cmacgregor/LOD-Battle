public enum HealthTier
{
    High,
    Mid,
    Low
}

public static class HealthTierCalculator
{
    public const decimal HighHealthPercentage = 2m / 3m;
    public const decimal LowHealthPercentage = 1m / 3m;

    public static HealthTier GetTier(decimal currentHealth, decimal maxHealth)
    {
        var healthPercentage = currentHealth / maxHealth;

        if (healthPercentage >= HighHealthPercentage)
        {
            return HealthTier.High;
        }

        if (healthPercentage <= LowHealthPercentage)
        {
            return HealthTier.Low;
        }

        return HealthTier.Mid;
    }
}
