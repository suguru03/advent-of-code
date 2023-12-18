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
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(Data data)
    {
        var grid = data.Grid;
        var ty = grid.Count - 1;
        var tx = grid[0].Count - 1;
        var memo = new Dictionary<(int x, int y), (int sum, Direction, int count)>
        {
            [(tx, ty)] = ((grid.Count + grid[0].Count) * 9, Direction.None, 0)
        };
        FindMin(grid, 0, 0, -grid[0][0], Direction.None, 0, memo, new HashSet<(int x, int y)>());
        return memo[(tx, ty)].sum;
    }

    private enum Direction
    {
        None,
        Downward,
        Left,
        Upward,
        Right,
    }

    private static readonly List<(Direction, int dx, int dy)> Deltas = new()
    {
        (Direction.Right, 1, 0),
        (Direction.Upward, 0, 1),
        (Direction.Left, -1, 0),
        (Direction.Downward, 0, -1),
    };

    private static bool IsValidRange(IList<List<int>> grid, int x, int y)
    {
        return y >= 0 && y < grid.Count && x >= 0 && x < grid[y].Count;
    }


    private static void FindMin(IList<List<int>> grid, int x, int y, int sum, Direction direction, int directionCount,
        IDictionary<(int x, int y), (int sum, Direction direction, int count)> memo,
        ISet<(int x, int y)> seen)
    {
        const int directionLimit = 3;
        var seenKey = (x, y);
        if (!IsValidRange(grid, x, y) || directionCount > directionLimit || seen.Contains(seenKey))
        {
            return;
        }

        sum += grid[y][x];
        var tx = grid[y].Count - 1;
        var ty = grid.Count - 1;
        var min = memo[(tx, ty)].sum;
        if (sum >= min)
        {
            return;
        }

        var memoKey = (x, y);
        var cache = (sum, direction, directionCount);
        if (memo.TryGetValue(memoKey, out var prev))
        {
            if (direction == prev.direction && directionCount == prev.count)
            {
                if (sum >= prev.sum)
                {
                    return;
                }
            }

            if (sum >= prev.sum + 9 * directionLimit / 2)
            {
                return;
            }

            if (sum < prev.sum)
            {
                memo[memoKey] = cache;
            }
        }
        else
        {
            memo[memoKey] = cache;
        }

        if (x == tx && y == ty)
        {
            return;
        }


        seen.Add(seenKey);

        foreach (var (nextDirection, dx, dy) in Deltas)
        {
            var nextCount = direction == nextDirection ? directionCount + 1 : 1;
            FindMin(grid, x + dx, y + dy, sum, nextDirection, nextCount, memo, seen);
        }

        seen.Remove(seenKey);
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseLines(useExample ? "example.txt" : "input.txt", text => text.Select(c => c - '0').ToList())
            .ToList());
    }

    private readonly record struct Data(List<List<int>> Grid);
}
