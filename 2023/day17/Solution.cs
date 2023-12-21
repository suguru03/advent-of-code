using advent_of_code.Common;

namespace advent_of_code._2023.day17;

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

    private static int Solve1(Data data)
    {
        return FindMin(data.Grid, 0, 3);
    }

    private static int Solve2(Data data)
    {
        return FindMin(data.Grid, 4, 10);
    }

    private enum Direction
    {
        None,
        Downward,
        Left,
        Upward,
        Right,
    }

    private static readonly Dictionary<Direction, ( int dx, int dy)> DeltaMap = new()
    {
        { Direction.Right, (1, 0) },
        { Direction.Upward, (0, 1) },
        { Direction.Left, (-1, 0) },
        { Direction.Downward, (0, -1) },
    };

    private static bool IsValidRange(IList<List<int>> grid, int x, int y)
    {
        return y >= 0 && y < grid.Count && x >= 0 && x < grid[y].Count;
    }

    private static int FindMin(IList<List<int>> grid, int minMove, int maxMove)
    {
        var queue =
            new PriorityQueue<(int x, int y, int sum, Direction direction, int count, HashSet<(int x, int y)> seen),
                int>();
        var memo = new Dictionary<(int x, int y, Direction direction, int count), int>();
        queue.Enqueue((0, 0, 0, Direction.None, minMove, new HashSet<(int x, int y)> { (0, 0) }), 0);
        var ty = grid.Count - 1;
        var tx = grid[ty].Count - 1;
        var min = (tx + ty) * 9;
        while (queue.Count != 0)
        {
            var (x, y, sum, direction, directionCount, seen) = queue.Dequeue();
            if (sum >= min)
            {
                continue;
            }

            if (x == tx && y == ty)
            {
                if (directionCount >= minMove)
                {
                    min = sum;
                }

                continue;
            }

            var memoKey = (x, y, direction, directionCount);
            if (memo.TryGetValue(memoKey, out var prev) && sum >= prev)
            {
                continue;
            }

            memo[memoKey] = sum;

            foreach (var (nextDirection, (dx, dy)) in DeltaMap)
            {
                if (directionCount < minMove && direction != nextDirection)
                {
                    continue;
                }

                var nx = x + dx;
                var ny = y + dy;
                var nextCount = direction == nextDirection ? directionCount + 1 : 1;
                if (!IsValidRange(grid, nx, ny) || nextCount > maxMove || seen.Contains((nx, ny)))
                {
                    continue;
                }

                var nextSum = sum + grid[ny][nx];
                var nextSeen = new HashSet<(int x, int y)>(seen) { (nx, ny) };
                queue.Enqueue((nx, ny, nextSum, nextDirection, nextCount, nextSeen), nextSum);
            }
        }

        return min;
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseLines(useExample ? "example.txt" : "input.txt", text => text.Select(c => c - '0').ToList())
            .ToList());
    }

    private readonly record struct Data(List<List<int>> Grid);
}
