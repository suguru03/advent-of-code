using advent_of_code.Common;

namespace advent_of_code._2024.day4;

public class Solution : SolutionBase
{
    private static readonly char[] Xmas = ['X', 'M', 'A', 'S'];
    private static readonly (int dx, int dy)[][] Deltas = [[(-1, -1), (1, 1)], [(1, -1), (-1, 1)]];

    public override object Run(int part, bool useExample)
    {
        var input = ParseLines(!useExample ? "input.txt" : "example.txt",
            row => row.ToCharArray()).ToArray();
        return part switch
        {
            1 => Solve1(input),
            2 => Solve2(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(char[][] grid)
    {
        var sum = 0;
        for (var y = 0; y < grid.Length; y++)
        for (var x = 0; x < grid[y].Length; x++)
        for (var dy = -1; dy <= 1; dy++)
        for (var dx = -1; dx <= 1; dx++)
        {
            if (dx == 0 && dy == 0)
            {
                continue;
            }

            sum += Convert.ToInt32(IsValid(grid, x, y, dx, dy, 0));
        }

        return sum;
    }

    private static bool IsValid(char[][] grid, int x, int y, int dx, int dy, int charIndex)
    {
        if (y < 0 || y >= grid.Length || x < 0 || x >= grid[y].Length)
        {
            return false;
        }

        if (grid[y][x] != Xmas[charIndex])
        {
            return false;
        }

        if (charIndex == Xmas.Length - 1)
        {
            return true;
        }

        return IsValid(grid, x + dx, y + dy, dx, dy, charIndex + 1);
    }

    private static int Solve2(char[][] grid)
    {
        var sum = 0;
        for (var y = 1; y < grid.Length - 1; y++)
        for (var x = 1; x < grid[y].Length - 1; x++)
        {
            sum += Convert.ToInt32(IsValidMas(grid, x, y));
        }

        return sum;
    }

    private static bool IsValidMas(char[][] grid, int x, int y)
    {
        return grid[y][x] == 'A' && Deltas.All(deltas => IsValidPair(grid, x, y, deltas));
    }

    private static bool IsValidPair(char[][] grid, int x, int y, (int dx, int dy)[] deltas)
    {
        return IsValidPair(deltas.Select(d => grid[y + d.dy][x + d.dx]).ToArray());
    }

    private static bool IsValidPair(char[] chars)
    {
        var c1 = chars.First();
        var c2 = chars.Last();
        return IsValidPair(c1, c2) || IsValidPair(c2, c1);
    }

    private static bool IsValidPair(char c1, char c2)
    {
        return c1 == 'M' && c2 == 'S';
    }
}
