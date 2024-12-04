using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2024.day2;

public partial class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var input = ParseLines(!useExample ? "input.txt" : part == 1 ? "example1.txt" : "example2.txt",
            row => row.Split(" ").Select(int.Parse).ToArray());
        return part switch
        {
            1 => Solve1(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IEnumerable<int[]> parseLines)
    {
        return parseLines.Count(row =>
        {
            var sign = Math.Sign(row[1] - row[0]);
            if (sign == 0)
            {
                return false;
            }

            for (var left = 0; left < row.Length - 1; left++)
            {
                var right = left + 1;
                if (Math.Sign(row[right] - row[left]) != sign)
                {
                    return false;
                }

                var diff = Math.Abs(row[right] - row[left]);
                if (diff is < 1 or > 3)
                {
                    return false;
                }
            }

            return true;
        });
    }
}
