using advent_of_code.Common;

namespace advent_of_code._2023.day9;

public class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var input = Parse(useExample);
        return part switch
        {
            1 => Solve1(input),
            2 => Solve2(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IEnumerable<IEnumerable<int>> data)
    {
        var sum = 0;
        foreach (var row in data)
        {
            var list = new List<List<int>> { row.ToList() };
            bool hasValue;
            do
            {
                hasValue = false;
                var prev = list.Last();
                var size = prev.Count - 1;
                var next = new List<int>(size);
                list.Add(next);
                for (var i = 0; i < size; i++)
                {
                    var diff = prev[i + 1] - prev[i];
                    hasValue |= diff != 0;
                    next.Add(diff);
                }
            } while (hasValue);

            for (var i = list.Count - 1; i >= 1; i--)
            {
                var prev = list[i - 1];
                var current = list[i];
                prev.Add(prev.Last() + current.Last());
            }

            sum += list.First().Last();
        }

        return sum;
    }

    private static int Solve2(IEnumerable<IEnumerable<int>> data)
    {
        return Solve1(data.ToList().Select(row => row.Reverse()));
    }

    private IEnumerable<IEnumerable<int>> Parse(bool useExample)
    {
        return ParseLines<IEnumerable<int>>(useExample ? "example.txt" : "input.txt",
            text => text.Split(" ").Select(int.Parse));
    }
}
