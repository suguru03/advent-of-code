using advent_of_code.Common;

namespace advent_of_code._2024.day2;

public class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var input = ParseLines(!useExample ? "input.txt" : "example.txt",
            row => row.Split(" ").Select(int.Parse).ToArray());
        return part switch
        {
            1 => Solve1(input),
            2 => Solve2(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IEnumerable<int[]> parseLines)
    {
        return parseLines.Count(IsSafe);
    }

    private static int Solve2(IEnumerable<int[]> parseLines)
    {
        return parseLines.Count(rows => IsSafe(rows) || rows.Where((_, i) => IsSafe(RemoveAt(rows, i))).Any());
    }

    private static bool IsSafe(int[] rows)
    {
        var sign = Math.Sign(rows[1] - rows[0]);
        if (sign == 0)
        {
            return false;
        }

        for (var left = 0; left < rows.Length - 1; left++)
        {
            var right = left + 1;
            if (Math.Sign(rows[right] - rows[left]) != sign)
            {
                return false;
            }

            var diff = Math.Abs(rows[right] - rows[left]);
            if (diff is < 1 or > 3)
            {
                return false;
            }
        }

        return true;
    }

    private static int[] RemoveAt(int[] rows, int index)
    {
        return rows.Take(index).Concat(rows.Skip(index + 1)).ToArray();
    }
}
