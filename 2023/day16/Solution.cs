using advent_of_code.Common;

namespace advent_of_code._2023.day16;

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

    private enum Direction
    {
        Upward,
        Left,
        Downward,
        Right,
    }

    private static readonly Dictionary<Direction, (int x, int y)> DeltaMap = new()
    {
        { Direction.Upward, (0, -1) },
        { Direction.Left, (-1, 0) },
        { Direction.Downward, (0, 1) },
        { Direction.Right, (1, 0) },
    };

    private static int Solve1(Data data)
    {
        return Traverse(data, 0, 0, Direction.Right);
    }

    private static int Solve2(Data data)
    {
        var grid = data.Grid;
        var max = 0;
        var right = grid[0].Count - 1;
        var bottom = data.Grid.Count - 1;
        for (var x = 0; x <= right; x++)
        {
            Resolve(Traverse(data, x, 0, Direction.Downward));
            Resolve(Traverse(data, x, bottom, Direction.Upward));
        }

        for (var y = 0; y <= bottom; y++)
        {
            Resolve(Traverse(data, 0, y, Direction.Right));
            Resolve(Traverse(data, right, y, Direction.Left));
        }

        return max;

        void Resolve(int value)
        {
            max = Math.Max(max, value);
        }
    }

    private static int Traverse(Data data, int x, int y, Direction startDirection)
    {
        var grid = data.Grid;
        var seen = grid.Select(row => row.Select(_ => new HashSet<Direction>()).ToList()).ToList();
        var delta = DeltaMap[startDirection];
        Traverse(grid, x - delta.x, y - delta.y, startDirection, seen);
        return seen.SelectMany(row => row).Count(set => set.Count != 0);
    }

    private static void Traverse(IReadOnlyList<List<char>> grid, int x, int y, Direction direction,
        IReadOnlyList<List<HashSet<Direction>>> seen)
    {
        var delta = DeltaMap[direction];
        x += delta.x;
        y += delta.y;
        if (y < 0 || y >= grid.Count || x < 0 || x >= grid[y].Count)
        {
            return;
        }

        if (seen[y][x].Contains(direction))
        {
            return;
        }

        seen[y][x].Add(direction);
        switch (grid[y][x], direction)
        {
            case ('|', Direction.Left):
            case ('|', Direction.Right):
                Traverse(grid, x, y, Direction.Upward, seen);
                Traverse(grid, x, y, Direction.Downward, seen);
                break;
            case ('-', Direction.Upward):
            case ('-', Direction.Downward):
                Traverse(grid, x, y, Direction.Left, seen);
                Traverse(grid, x, y, Direction.Right, seen);
                break;
            case ('/', Direction.Upward):
            case ('\\', Direction.Downward):
                Traverse(grid, x, y, Direction.Right, seen);
                break;
            case ('/', Direction.Left):
            case ('\\', Direction.Right):
                Traverse(grid, x, y, Direction.Downward, seen);
                break;
            case ('/', Direction.Downward):
            case ('\\', Direction.Upward):
                Traverse(grid, x, y, Direction.Left, seen);
                break;
            case ('/', Direction.Right):
            case ('\\', Direction.Left):
                Traverse(grid, x, y, Direction.Upward, seen);
                break;
            default:
                Traverse(grid, x, y, direction, seen);
                break;
        }
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseLines(useExample ? "example.txt" : "input.txt", text => text.ToList()).ToList());
    }

    private readonly record struct Data(List<List<char>> Grid);
}
