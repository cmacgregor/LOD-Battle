using NUnit.Framework;

public class StatClampTests
{
    [Test]
    public void Clamp_ValueWithinRange_ReturnsValueUnchanged()
    {
        Assert.AreEqual(15, StatClamp.Clamp(15, 30));
    }

    [Test]
    public void Clamp_ValueBelowZero_ReturnsZero()
    {
        Assert.AreEqual(0, StatClamp.Clamp(-5, 30));
    }

    [Test]
    public void Clamp_ValueAboveMax_ReturnsMax()
    {
        Assert.AreEqual(30, StatClamp.Clamp(999, 30));
    }

    [Test]
    public void Clamp_ValueAtBounds_ReturnsValueUnchanged()
    {
        Assert.AreEqual(0, StatClamp.Clamp(0, 30));
        Assert.AreEqual(30, StatClamp.Clamp(30, 30));
    }
}
