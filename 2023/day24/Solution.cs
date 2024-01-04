using System.Text.RegularExpressions;
using advent_of_code.Common;
using advent_of_code.Common.utils;

namespace advent_of_code._2023.day24;

public partial class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var input = Parse(useExample);
        return part switch
        {
            1 => Solve1(input,
                useExample ? new Range<double>(7, 27) : new Range<double>(200000000000000, 400000000000000)),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(Data data, Range<double> range)
    {
        var result = 0;
        var hailstones = data.Hailstones;
        for (var i = 0; i < hailstones.Count - 1; i++)
        for (var j = i + 1; j < hailstones.Count; j++)
        {
            var h1 = hailstones[i];
            var h2 = hailstones[j];
            var x = (h2.Intersect - h1.Intersect) / (h1.Slope - h2.Slope);
            var y = h1.Slope * x + h1.Intersect;
            if (Math.Sign(h1.Velocity.X) > 0 ? x < h1.Position.X : x > h1.Position.X)
            {
                continue;
            }

            if (Math.Sign(h2.Velocity.X) > 0 ? x < h2.Position.X : x > h2.Position.X)
            {
                continue;
            }

            result += Convert.ToInt32(range.Contains(x) && range.Contains(y));
        }

        return result;
    }

    private Data Parse(bool useExample)
    {
        var rowRegex = RowRegex();
        return new Data(ParseLines(useExample ? "example.txt" : "input.txt", text =>
        {
            var group = rowRegex.Match(text).Groups;
            return new Hailstone(Parse3D(group["Position"].Value), Parse3D(group["Velocity"].Value));
        }).ToList());
    }

    private static Vector3D Parse3D(string text)
    {
        var nums = text.Trim().Split(',').Select(str => long.Parse(str.Trim())).ToList();
        return new Vector3D(nums[0], nums[1], nums[2]);
    }

    private record Data(List<Hailstone> Hailstones);

    private class Hailstone
    {
        public readonly Vector3D Position;
        public readonly Vector3D Velocity;
        public readonly double Slope;
        public readonly double Intersect;

        public Hailstone(Vector3D position, Vector3D velocity)
        {
            Position = position;
            Velocity = velocity;
            Slope = (double)velocity.Y / velocity.X;
            Intersect = position.Y - Slope * position.X;
        }
    }

    private record Vector3D(long X, long Y, long Z);

    [GeneratedRegex(@"(?<Position>-?.+) @ (?<Velocity>-?.+)")]
    private static partial Regex RowRegex();
}
