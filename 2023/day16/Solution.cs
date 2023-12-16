using advent_of_code.Common;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace advent_of_code._2023.day16;

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

    private enum Direction
    {
        Top,
        Left,
        Bottom,
        Right,
    }

    private static readonly Dictionary<Direction, (int x, int y)> DeltaMap = new()
    {
        { Direction.Top, (0, -1) },
        { Direction.Left, (-1, 0) },
        { Direction.Bottom, (0, 1) },
        { Direction.Right, (1, 0) },
    };

    private static int Solve1(Data data)
    {
        var grid = data.Grid;
        var seen = grid.Select(row => row.Select(_ => new HashSet<Direction>()).ToList()).ToList();
        Traverse(grid, -1, 0, Direction.Right, seen);
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
        switch (grid[y][x])
        {
            case '|':
            {
                if (direction is Direction.Left or Direction.Right)
                {
                    Traverse(grid, x, y, Direction.Top, seen);
                    Traverse(grid, x, y, Direction.Bottom, seen);
                    return;
                }

                break;
            }
            case '-':
            {
                if (direction is Direction.Top or Direction.Bottom)
                {
                    Traverse(grid, x, y, Direction.Left, seen);
                    Traverse(grid, x, y, Direction.Right, seen);
                    return;
                }

                break;
            }

            case '/':
            {
                var nextDirection = direction switch
                {
                    Direction.Top => Direction.Right,
                    Direction.Left => Direction.Bottom,
                    Direction.Bottom => Direction.Left,
                    Direction.Right => Direction.Top,
                    _ => throw new ArgumentOutOfRangeException(nameof(direction))
                };
                Traverse(grid, x, y, nextDirection, seen);
                return;
            }
            case '\\':
            {
                var nextDirection = direction switch
                {
                    Direction.Top => Direction.Left,
                    Direction.Left => Direction.Top,
                    Direction.Bottom => Direction.Right,
                    Direction.Right => Direction.Bottom,
                    _ => throw new ArgumentOutOfRangeException(nameof(direction))
                };
                Traverse(grid, x, y, nextDirection, seen);
                return;
            }
        }

        Traverse(grid, x, y, direction, seen);
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseLines(useExample ? "example.txt" : "input.txt", text => text.ToList()).ToList());
    }

    private readonly record struct Data(List<List<char>> Grid);
}
