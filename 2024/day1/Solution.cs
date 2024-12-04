using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2024.day1;

public partial class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var regex = InputRegex();
        var input = ParseLines(!useExample ? "input.txt" : part == 1 ? "example1.txt" : "example2.txt",
            row =>
            {
                var groups = regex.Match(row).Groups;
                return (int.Parse(groups[1].Value), int.Parse(groups[2].Value));
            });
        return part switch
        {
            1 => Solve1(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IEnumerable<(int, int)> parseLines)
    {
        var lefts = new List<int>();
        var rights = new List<int>();
        foreach (var (left, right) in parseLines)
        {
            lefts.Add(left);
            rights.Add(right);
        }

        lefts.Sort();
        rights.Sort();

        return lefts.Zip(rights, (x, y) => Math.Abs(x - y)).Sum();
    }

    [GeneratedRegex(@"(\d+)\s+(\d+)")]
    private static partial Regex InputRegex();
}
