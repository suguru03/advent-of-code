using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2024.day3;

public partial class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        return part switch
        {
            1 => Solve1(useExample),
            2 => Solve2(useExample),
            _ => ProblemNotSolvedString
        };
    }

    private int Solve1(bool useExample)
    {
        var inputRegex = Input1Regex();
        return ParseText(!useExample ? "input.txt" : "example1.txt", row => inputRegex.Matches(row))
            .Select(m => (left: int.Parse(m.Groups[1].Value), right: int.Parse(m.Groups[2].Value)))
            .Sum(tuple => tuple.left * tuple.right);
    }

    [GeneratedRegex(@"mul\((\d+),(\d+)\)")]
    private static partial Regex Input1Regex();

    private int Solve2(bool useExample)
    {
        var inputRegex = Input2Regex();
        var matches = ParseText(!useExample ? "input.txt" : "example2.txt", row => inputRegex.Matches(row));
        var sum = 0;
        var enabled = true;
        foreach (Match match in matches)
        {
            switch (match.Value)
            {
                case "do()":
                {
                    enabled = true;
                    break;
                }
                case "don't()":
                {
                    enabled = false;
                    break;
                }
                default:
                {
                    if (!enabled)
                    {
                        break;
                    }

                    sum += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
                    break;
                }
            }
        }

        return sum;
    }

    [GeneratedRegex(@"mul\((\d+),(\d+)\)|do(n\'t)?\(\)")]
    private static partial Regex Input2Regex();
}
