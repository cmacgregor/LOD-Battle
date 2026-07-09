using UnityEngine;

public static class SpiritBarMath
{
    private const int PointsPerBar = 100;

    public static int GetBarCount(int currentSpirit)
    {
        return currentSpirit / PointsPerBar;
    }

    public static int GetBarLevel(int currentSpirit)
    {
        return currentSpirit % PointsPerBar;
    }

    public static Color GetBarColor(int barCount)
    {
        switch (barCount)
        {
            case 1:
                return Color.blue;
            case 2:
                return Color.green;
            case 3:
                return Color.yellow;
            case 4:
                return new Color(1f, 165f / 255f, 0f); //orange
            case 5:
                return Color.red;
            default:
                return Color.clear;
        }
    }
}
