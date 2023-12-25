namespace advent_of_code._2023.Utils;

public static class Math
{
    private static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }

    public static long Lcm(long a, long b)
    {
        return a * b / Gcd(a, b);
    }

    public static int BitCount(int x)
    {
        x = ((x >> 1) & 0x55555555) + (x & 0x55555555);
        x = ((x >> 2) & 0x33333333) + (x & 0x33333333);
        x = ((x >> 4) & 0x0f0f0f0f) + (x & 0x0f0f0f0f);
        x = ((x >> 8) & 0x00ff00ff) + (x & 0x00ff00ff);
        return ((x >> 16) & 0x0000ffff) + (x & 0x0000ffff);
    }

}
