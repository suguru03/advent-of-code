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
            2 => Solve2(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IEnumerable<Data> list)
    {
        return Solve(list, FindPerfectMirrorIndex);
    }

    private static int Solve2(IEnumerable<Data> list)
    {
        return Solve(list, FindPerfectMirrorWithSmudgeIndex);
    }

    private static int Solve(IEnumerable<Data> list, Func<IList<int>, int> finder)
    {
        return list.Select(data =>
        {
            var grid = data.Grid;
            var rows = grid.Select(row => row.Aggregate(0, (cur, c) => (cur << 1) + Convert.ToInt32(c == '#')))
                .ToList();
            var index = finder(rows);
            if (index != -1)
            {
                return (index + 1) * 100;
            }

            var columns = grid.First().Select((_, x) =>
                grid.Aggregate(0, (cur, row) => (cur << 1) + Convert.ToInt32(row[x] == '#'))).ToList();
            return finder(columns) + 1;
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

    private static int FindPerfectMirrorWithSmudgeIndex(IList<int> list)
    {
        for (var i = 1; i < list.Count; i++)
        {
            var left = i - 1;
            var right = i;
            var smudge = false;
            while (list[left] == list[right] || !smudge && (smudge = HasSmudge(list[left], list[right])))
            {
                if (--left >= 0 && ++right < list.Count)
                {
                    continue;
                }

                if (!smudge)
                {
                    break;
                }

                return i - 1;
            }
        }

        return -1;
    }

    private static bool HasSmudge(int a, int b)
    {
        return BitCount(a ^ b) == 1;
    }

    private static int BitCount(int x)
    {
        x = ((x >> 1) & 0x55555555) + (x & 0x55555555);
        x = ((x >> 2) & 0x33333333) + (x & 0x33333333);
        x = ((x >> 4) & 0x0f0f0f0f) + (x & 0x0f0f0f0f);
        x = ((x >> 8) & 0x00ff00ff) + (x & 0x00ff00ff);
        return ((x >> 16) & 0x0000ffff) + (x & 0x0000ffff);
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
