using System.Diagnostics;
using advent_of_code.Common;

namespace advent_of_code._2023.day14;

public class Solution : SolutionBase
{
    private const char Rock = 'O';
    private const char Wall = '#';
    private const char Empty = '.';

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

    private static int Solve1(Data data)
    {
        var grid = data.Grid;
        var sum = 0;
        var top = 0;
        var width = grid.First().Count;
        for (var x = 0; x < width; x++, top = 0)
        for (var y = 0; y < grid.Count; y++)
        {
            switch (grid[y][x])
            {
                case Rock:
                    sum += width - top++;
                    break;
                case Wall:
                    top = y + 1;
                    break;
                case Empty:
                    break;
            }
        }

        return sum;
    }

    private static int Solve2(Data data)
    {
        var grid = data.Grid;
        var snapshotMap = new Dictionary<string, int>();
        var count = -1;
        const int limit = 1_000_000_000;
        while (++count < limit)
        {
            Cycle(grid);
            var snapshot = Snapshot(grid);
            if (snapshotMap.TryAdd(snapshot, count))
            {
                continue;
            }

            var loopStart = snapshotMap[snapshot];
            var loopSize = count - loopStart;
            var n = (limit - loopStart) / loopSize;
            var index = limit - loopSize * n - 1;
            var target = snapshotMap.ToList().Find(kvp => kvp.Value == index).Key;
            return Count(target);
        }

        return 0;
    }

    private static string Snapshot(IEnumerable<List<char>> grid)
    {
        return string.Join('\n', grid.Select(row => string.Join("", row)));
    }

    private static void Print(IEnumerable<List<char>> grid)
    {
        Console.WriteLine($"{Snapshot(grid)}\n");
    }

    private static int Count(string key)
    {
        var grid = key.Split('\n');
        return grid.Select((row, y) => row.Sum(c => c == Rock ? grid.Length - y : 0)).Sum();
    }

    private static void Cycle(IReadOnlyList<List<char>> grid)
    {
        MoveTo(grid, 0, -1);
        MoveTo(grid, -1, 0);
        MoveTo(grid, 0, 1);
        MoveTo(grid, 1, 0);
    }

    private static void MoveTo(IReadOnlyList<List<char>> grid, int dx, int dy)
    {
        var width = grid[0].Count;
        var depth = grid.Count;
        var sign = -1 * (Math.Sign(dx) + Math.Sign(dy));
        var x = sign == 1 ? -1 : width;
        while ((x += sign) >= 0 && x < width)
        {
            var y = sign == 1 ? -1 : depth;
            while ((y += sign) >= 0 && y < depth)
            {
                if (grid[y][x] != Rock)
                {
                    continue;
                }

                var tx = x;
                var ty = y;
                while ((tx += dx) >= 0 && tx < width && grid[y][tx] == Empty)
                {
                }

                while ((ty += dy) >= 0 && ty < depth && grid[ty][x] == Empty)
                {
                }

                tx -= dx;
                ty -= dy;
                if (y == ty && x == tx)
                {
                    continue;
                }

                grid[ty][tx] = Rock;
                grid[y][x] = Empty;
            }
        }
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseLines(useExample ? "example.txt" : "input.txt", text => text.ToList()).ToList());
    }

    private record Data(List<List<char>> Grid);
}
