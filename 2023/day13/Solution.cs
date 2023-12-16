using System.Diagnostics;
using advent_of_code.Common;

namespace advent_of_code._2023.day13;

public class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var input = Parse(useExample);
        return part switch
        {
            1 => Solve1(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IEnumerable<Data> list)
    {
        return list.Select(data =>
        {
            var grid = data.Grid;
            var str = string.Join('\n', grid.Select(r => new string(r.ToArray())));
            var rows = grid.Select(row => row.Aggregate(0, (cur, c) => (cur << 1) + Convert.ToInt32(c == '#')))
                .ToList();
            var index = FindPerfectMirrorIndex(rows);
            if (index != -1)
            {
                return (index + 1) * 100;
            }

            var columns = grid.First().Select((_, x) =>
                grid.Aggregate(0, (cur, row) => (cur << 1) + Convert.ToInt32(row[x] == '#'))).ToList();
            return FindPerfectMirrorIndex(columns) + 1;
        }).Sum();
    }

    private static int FindPerfectMirrorIndex(IList<int> list)
    {
        for (var i = 1; i < list.Count; i++)
        {
            var left = i - 1;
            var right = i;
            while (list[left] == list[right])
            {
                if (--left >= 0 && ++right < list.Count)
                {
                    continue;
                }

                return i - 1;
            }
        }

        return -1;
    }

    private IEnumerable<Data> Parse(bool useExample)
    {
        var rows = ParseLines(useExample ? "example.txt" : "input.txt", text => text);
        var items = new List<Data>();
        var grid = new List<List<char>>();
        foreach (var row in rows)
        {
            if (string.IsNullOrEmpty(row))
            {
                items.Add(new Data(grid));
                grid = new List<List<char>>();
                continue;
            }

            grid.Add(row.ToList());
        }

        items.Add(new Data(grid));
        return items;
    }

    private readonly record struct Data(List<List<char>> Grid);
}
