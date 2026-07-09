using NUnit.Framework;

public class HealthTierCalculatorTests
{
    [TestCase(30, 30)] // 100%
    [TestCase(20, 30)] // ~66.7%, at the high threshold
    [TestCase(30, 45)] // exactly 2/3
    public void GetTier_AtOrAboveTwoThirds_ReturnsHigh(decimal currentHealth, decimal maxHealth)
    {
        Assert.AreEqual(HealthTier.High, HealthTierCalculator.GetTier(currentHealth, maxHealth));
    }

    [TestCase(15, 30)] // 50%
    [TestCase(11, 30)] // just above 1/3
    public void GetTier_BetweenOneThirdAndTwoThirds_ReturnsMid(decimal currentHealth, decimal maxHealth)
    {
        Assert.AreEqual(HealthTier.Mid, HealthTierCalculator.GetTier(currentHealth, maxHealth));
    }

    [TestCase(10, 30)] // exactly 1/3
    [TestCase(0, 30)]  // downed
    public void GetTier_AtOrBelowOneThird_ReturnsLow(decimal currentHealth, decimal maxHealth)
    {
        Assert.AreEqual(HealthTier.Low, HealthTierCalculator.GetTier(currentHealth, maxHealth));
    }
}
