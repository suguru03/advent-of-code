using System.Numerics;

namespace advent_of_code.Common.utils;

public record Range<T>(T Min, T Max) where T : IBinaryNumber<T>
{
    public bool Contains(T value)
    {
        return value >= Min && value <= Max;
    }
}
