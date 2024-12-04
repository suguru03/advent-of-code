using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2024.day3;

public partial class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var inputRegex = InputRegex();
        var input = ParseText(!useExample ? "input.txt" : "example.txt",
            row => inputRegex.Matches(row).Select(m => (int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value))));
        return part switch
        {
            1 => Solve1(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IEnumerable<(int left, int right)> rows)
    {
        return rows.Sum(tuple => tuple.left * tuple.right);
    }

    [GeneratedRegex(@"mul\((\d+),(\d+)\)")]
    private static partial Regex InputRegex();
}
