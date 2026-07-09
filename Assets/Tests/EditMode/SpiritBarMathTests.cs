using NUnit.Framework;
using UnityEngine;

public class SpiritBarMathTests
{
    [TestCase(0, 0)]
    [TestCase(35, 0)]
    [TestCase(99, 0)]
    [TestCase(100, 1)]
    [TestCase(150, 1)]
    [TestCase(200, 2)]
    public void GetBarCount_ReturnsCompletedBars(int currentSpirit, int expectedBarCount)
    {
        Assert.AreEqual(expectedBarCount, SpiritBarMath.GetBarCount(currentSpirit));
    }

    [TestCase(0, 0)]
    [TestCase(35, 35)]
    [TestCase(99, 99)]
    [TestCase(100, 0)]
    [TestCase(150, 50)]
    public void GetBarLevel_ReturnsProgressWithinCurrentBar(int currentSpirit, int expectedBarLevel)
    {
        Assert.AreEqual(expectedBarLevel, SpiritBarMath.GetBarLevel(currentSpirit));
    }

    [Test]
    public void GetBarColor_ZeroBars_ReturnsClear()
    {
        Assert.AreEqual(Color.clear, SpiritBarMath.GetBarColor(0));
    }

    [TestCase(1, 0f, 0f, 1f)]
    [TestCase(2, 0f, 1f, 0f)]
    [TestCase(3, 1f, 1f, 0f)]
    [TestCase(5, 1f, 0f, 0f)]
    public void GetBarColor_KnownLevels_ReturnExpectedColor(int barCount, float r, float g, float b)
    {
        var color = SpiritBarMath.GetBarColor(barCount);

        Assert.AreEqual(r, color.r, 0.001f);
        Assert.AreEqual(g, color.g, 0.001f);
        Assert.AreEqual(b, color.b, 0.001f);
    }

    [Test]
    public void GetBarColor_LevelFour_IsOrangeNotHsvGarbage()
    {
        var color = SpiritBarMath.GetBarColor(4);

        Assert.AreEqual(1f, color.r, 0.001f);
        Assert.AreEqual(165f / 255f, color.g, 0.001f);
        Assert.AreEqual(0f, color.b, 0.001f);
    }

    [Test]
    public void GetBarColor_UnknownLevel_ReturnsClear()
    {
        Assert.AreEqual(Color.clear, SpiritBarMath.GetBarColor(6));
    }
}
