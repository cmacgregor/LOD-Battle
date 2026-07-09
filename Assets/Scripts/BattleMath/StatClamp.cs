public static class StatClamp
{
    public static int Clamp(int value, int max)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }
}
