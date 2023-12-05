using advent_of_code.Common;

namespace advent_of_code._2023.day1;

public class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var input = ParseLines(!useExample ? "input.txt" : part == 1 ? "example1.txt" : "example2.txt", row => row);
        return part switch
        {
            1 => Solve1(input),
            2 => Solve2(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IEnumerable<string> parseLines)
    {
        var iter = (char c) => int.TryParse(c.ToString(), out _);
        return parseLines.Aggregate(0,
            ((acc, row) => acc + int.Parse(row.First(iter).ToString()) * 10 + int.Parse(row.Last(iter).ToString())));
    }

    private static object Solve2(IEnumerable<string> parseLines)
    {
        var (leftMap, rightMap) = ConstructMap();
        return parseLines.Aggregate(0, (acc, row) =>
        {
            var l = row.Length;
            var left = "";
            for (var i = 0; i < l; i++)
            {
                left += row[i];
                while (left.Length > 0 && !leftMap.ContainsKey(left))
                {
                    left = left[1..];
                }

                if (!leftMap.TryGetValue(left, out var num) || num == 0)
                {
                    continue;
                }

                acc += num * 10;
                break;
            }

            var right = "";
            for (var i = l - 1; i >= 0; i--)
            {
                right = $"{row[i]}{right}";
                while (right.Length > 0 && !rightMap.ContainsKey(right))
                {
                    right = right[..^1];
                }

                if (!rightMap.TryGetValue(right, out var num) || num == 0)
                {
                    continue;
                }

                acc += num;
                break;
            }

            return acc;
        });
    }

    private static (Dictionary<string, int>, Dictionary<string, int>) ConstructMap()
    {
        var numMap = Enumerable.Range(1, 9).ToDictionary(num => num.ToString(), num => num);
        var leftMap = new Dictionary<string, int>(numMap);
        var rightMap = new Dictionary<string, int>(numMap);
        var arr = new[] { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        for (var num = 1; num < arr.Length; num++)
        {
            var str = arr[num];
            var left = "";
            var right = "";
            var l = str.Length;
            for (var i = 0; i < l; i++)
            {
                left += str[i];
                right = $"{str[l - i - 1]}{right}";
                leftMap[left] = 0;
                rightMap[right] = 0;
            }

            leftMap[left] = num;
            rightMap[right] = num;
        }

        return (leftMap, rightMap);
    }
}
