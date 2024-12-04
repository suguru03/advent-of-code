using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2024.day4;

public class Solution : SolutionBase
{
    private static readonly char[] Chars = ['X', 'M', 'A', 'S'];

    public override object Run(int part, bool useExample)
    {
        var input = ParseLines(!useExample ? "input.txt" : "example.txt",
            row => row.ToCharArray()).ToArray();
        return part switch
        {
            1 => Solve1(input),
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

            sum += IsValid(grid, x, y, dx, dy, 0);
        }

        return sum;
    }

    private static int IsValid(char[][] grid, int x, int y, int dx, int dy, int charIndex)
    {
        if (y < 0 || y >= grid.Length || x < 0 || x >= grid[y].Length)
        {
            return 0;
        }

        if (grid[y][x] != Chars[charIndex])
        {
            return 0;
        }

        if (charIndex == Chars.Length - 1)
        {
            return 1;
        }

        return IsValid(grid, x + dx, y + dy, dx, dy, charIndex + 1);
    }
}
