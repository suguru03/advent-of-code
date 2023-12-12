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
}
