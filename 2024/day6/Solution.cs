using advent_of_code.Common;

namespace advent_of_code._2024.day6;

public class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var grid = ParseLines(!useExample ? "input.txt" : "example.txt",
            row => row.ToCharArray()).ToArray();
        return part switch
        {
            1 => Solve1(grid),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(char[][] grid)
    {
        var (x, y, direction) = FindGuard(grid);
        return Count(grid, x, y, direction);
    }

    private static (int x, int y, Direction) FindGuard(char[][] grid)
    {
        for (var y = 0; y < grid.Length; y++)
        for (var x = 0; x < grid[y].Length; x++)
        {
            Direction? direction = grid[y][x] switch
            {
                '^' => Direction.Up,
                '>' => Direction.Right,
                'v' => Direction.Down,
                '<' => Direction.Left,
                _ => null
            };

            if (direction == null)
            {
                continue;
            }

            grid[y][x] = '.';
            return (x, y, direction.Value);
        }

        throw new Exception("Guard not found");
    }

    private static int Count(char[][] grid, int x, int y, Direction direction)
    {
        var count = 1;
        while (true)
        {
            var next = GetNextCoordinate(x, y, direction);
            if (!IsValidRange(grid, next.x, next.y))
            {
                break;
            }

            if (IsObstruction(grid, next.x, next.y))
            {
                direction = direction switch
                {
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up,
                    Direction.Up => Direction.Right,
                    _ => throw new Exception("Invalid direction")
                };
                continue;
            }

            if (grid[y][x] == '.')
            {
                count++;
                grid[y][x] = 'x';
            }

            x = next.x;
            y = next.y;
        }

        return count;
    }

    private static bool IsValidRange(char[][] grid, int x, int y)
    {
        return y >= 0 && y < grid.Length && x >= 0 && x < grid[y].Length;
    }

    private static bool IsObstruction(char[][] grid, int x, int y)
    {
        return grid[y][x] == '#';
    }

    private static (int x, int y) GetNextCoordinate(int x, int y, Direction direction)
    {
        var d = deltaMap[direction];
        return (x + d.dx, y + d.dy);
    }


    private enum Direction
    {
        Right,
        Down,
        Left,
        Up
    }

    private static readonly Dictionary<Direction, (int dx, int dy)> deltaMap = new()
    {
        { Direction.Right, (1, 0) },
        { Direction.Down, (0, 1) },
        { Direction.Left, (-1, 0) },
        { Direction.Up, (0, -1) }
    };
}
