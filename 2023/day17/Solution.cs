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
        var queue = new PriorityQueue<(State, int count), int>([((new State(0, 0, Direction.None, maxMove), 0), 0)]);
        var costMap = new Dictionary<State, int>();
        var goal = (x: grid.First().Count - 1, y: grid.Count - 1);
        while (queue.Count != 0)
        {
            var (state, cost) = queue.Dequeue();
            if (state.X == goal.x && state.Y == goal.y)
            {
                if (state.Count < minMove)
                {
                    continue;
                }

                return cost;
            }

            if (costMap.TryGetValue(state, out var prev) && cost >= prev)
            {
                continue;
            }

            costMap[state] = cost;

            foreach (var next in GetAdjacent(state, minMove, maxMove))
            {
                if (!IsValidRange(grid, next.X, next.Y))
                {
                    continue;
                }

                var nextSum = cost + grid[next.Y][next.X];
                queue.Enqueue((next, nextSum), nextSum);
            }
        }

        return -1;
    }

    private static IEnumerable<State> GetAdjacent(State state, int min, int max)
    {
        var (x, y, direction, count) = state;
        var current = direction == Direction.None ? (dx: 0, dy: 0) : DeltaMap[direction];
        if (count >= min)
        {
            foreach (var (nextDirection, (dx, dy)) in DeltaMap)
            {
                if ((current.dx ^ dx) != 1 && (current.dy ^ dy) != 1)
                {
                    continue;
                }

                yield return new State(x + dx, y + dy, nextDirection, 1);
            }
        }

        if (count < max)
        {
            yield return new State(x + current.dx, y + current.dy, direction, count + 1);
        }
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseLines(useExample ? "example.txt" : "input.txt", text => text.Select(c => c - '0').ToList())
            .ToList());
    }

    private record Data(List<List<int>> Grid);

    private record State(int X, int Y, Direction Direction, int Count);
}
