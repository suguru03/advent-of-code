using advent_of_code.Common;
using advent_of_code.Common.utils;

namespace advent_of_code._2023.day23;

public class Solution : SolutionBase
{
    private const char Rock = '#';

    public override object Run(int part, bool useExample)
    {
        var input = Parse(useExample);
        return part switch
        {
            1 => Solve1(input),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IReadOnlyList<List<char>> grid)
    {
        return Traverse(grid, new Vector2D(1, 0), new HashSet<Vector2D>(), new Dictionary<Vector2D, int>(), 0);
    }

    private static int Traverse(IReadOnlyList<List<char>> grid, Vector2D state, ISet<Vector2D> seen,
        IDictionary<Vector2D, int> maxMap, int step)
    {
        var (x, y) = state;
        if (y < 0 || y >= grid.Count || x < 0 || x >= grid[y].Count)
        {
            return step - 1;
        }

        if (grid[y][x] == Rock || seen.Contains(state) || maxMap.TryGetValue(state, out var max) && step < max)
        {
            return 0;
        }

        maxMap[state] = step++;
        seen.Add(state);
        max = GetAdjacent(grid, state).Max(next => Traverse(grid, next, seen, maxMap, step));
        seen.Remove(state);
        return max;
    }

    private static IEnumerable<Vector2D> GetAdjacent(IReadOnlyList<List<char>> grid, Vector2D state)
    {
        switch (grid[state.Y][state.X])
        {
            case '<':
                yield return state + Vector2D.Left;
                break;
            case '>':
                yield return state + Vector2D.Right;
                break;
            case '^':
                yield return state + Vector2D.Down;
                break;
            case 'v':
                yield return state + Vector2D.Up;
                break;
            default:
                foreach (var adjacent in state.GetAdjacentSet())
                {
                    yield return adjacent;
                }

                break;
        }
    }

    private List<List<char>> Parse(bool useExample)
    {
        return ParseLines(useExample ? "example.txt" : "input.txt", text => text.ToList()).ToList();
    }
}
