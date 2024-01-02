using advent_of_code.Common;
using advent_of_code.Common.utils;

namespace advent_of_code._2023.day21;

public class Solution : SolutionBase
{
    private const char Start = 'S';
    private const char Rock = '#';

    public override object Run(int part, bool useExample)
    {
        var input = Parse(useExample);
        return part switch
        {
            1 => Solve1(input, useExample ? 6 : 64),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IReadOnlyList<List<char>> grid, int targetStep)
    {
        var start = FindStart(grid);
        var queue = new Queue<Vector2D>(collection: [start]);
        var step = 0;
        var nextPlotSet = new HashSet<Vector2D>();
        while (step++ != targetStep)
        {
            nextPlotSet.Clear();
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                foreach (var next in current.GetAdjacentSet())
                {
                    var (x, y) = next;
                    if (y < 0 || y >= grid.Count || x < 0 || x >= grid[y].Count || grid[y][x] == Rock)
                    {
                        continue;
                    }

                    nextPlotSet.Add(next);
                }
            }

            foreach (var next in nextPlotSet)
            {
                queue.Enqueue(next);
            }
        }

        return nextPlotSet.Count;
    }

    private static Vector2D FindStart(IReadOnlyList<List<char>> grid)
    {
        for (var y = 0; y < grid.Count; y++)
        for (var x = 0; x < grid[y].Count; x++)
        {
            if (grid[y][x] == Start)
            {
                return new Vector2D(x, y);
            }
        }

        throw new Exception();
    }

    private List<List<char>> Parse(bool useExample)
    {
        return ParseLines(useExample ? "example.txt" : "input.txt", text => text.ToList()).ToList();
    }
}
