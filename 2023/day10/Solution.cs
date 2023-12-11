using advent_of_code.Common;

namespace advent_of_code._2023.day10;

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

    private static int Solve1(IReadOnlyList<char[]> grid)
    {
        var depth = -1;
        var queue = new Queue<((int x, int y) from, (int x, int y) current)>();
        queue.Enqueue(((-1, -1), FindStart(grid)));
        while (queue.Count > 0)
        {
            var next = new List<((int x, int y) current, (int x, int y) next)>();
            while (queue.Count > 0)
            {
                var (from, cur) = queue.Dequeue();
                if (!ValidPosition(grid, cur.x, cur.y))
                {
                    continue;
                }

                switch (grid[cur.y][cur.x])
                {
                    case 'S':
                    {
                        next.Add((cur, (cur.x - 1, cur.y)));
                        next.Add((cur, (cur.x + 1, cur.y)));
                        next.Add((cur, (cur.x, cur.y - 1)));
                        next.Add((cur, (cur.x, cur.y + 1)));
                        break;
                    }
                    case '|':
                    {
                        if (from.x != cur.x)
                        {
                            continue;
                        }

                        next.Add((cur, (cur.x, cur.y - 1)));
                        next.Add((cur, (cur.x, cur.y + 1)));
                        break;
                    }
                    case '-':
                    {
                        if (from.y != cur.y)
                        {
                            continue;
                        }

                        next.Add((cur, (cur.x - 1, cur.y)));
                        next.Add((cur, (cur.x + 1, cur.y)));
                        break;
                    }
                    case 'L':
                    {
                        if (cur.x + 1 != from.x && cur.y - 1 != from.y)
                        {
                            continue;
                        }

                        next.Add((cur, (cur.x + 1, cur.y)));
                        next.Add((cur, (cur.x, cur.y - 1)));
                        break;
                    }
                    case 'J':
                    {
                        if (cur.x - 1 != from.x && cur.y - 1 != from.y)
                        {
                            continue;
                        }

                        next.Add((cur, (cur.x - 1, cur.y)));
                        next.Add((cur, (cur.x, cur.y - 1)));
                        break;
                    }
                    case '7':
                    {
                        if (cur.x - 1 != from.x && cur.y + 1 != from.y)
                        {
                            continue;
                        }

                        next.Add((cur, (cur.x - 1, cur.y)));
                        next.Add((cur, (cur.x, cur.y + 1)));
                        break;
                    }
                    case 'F':
                    {
                        if (cur.x + 1 != from.x && cur.y + 1 != from.y)
                        {
                            continue;
                        }

                        next.Add((cur, (cur.x + 1, cur.y)));
                        next.Add((cur, (cur.x, cur.y + 1)));
                        break;
                    }
                }

                grid[cur.y][cur.x] = '.';
            }

            if (next.Count > 0)
            {
                depth++;
            }

            foreach (var item in next)
            {
                queue.Enqueue(item);
            }
        }

        return depth;
    }

    private static (int, int) FindStart(IReadOnlyList<char[]> grid)
    {
        for (var y = 0; y < grid.Count; y++)
        {
            for (var x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x] == 'S')
                {
                    return (x, y);
                }
            }
        }

        return (-1, -1);
    }

    private static bool ValidPosition(IReadOnlyList<char[]> grid, int x, int y)
    {
        if (y < 0 || y >= grid.Count || x < 0 || x >= grid[y].Length)
        {
            return false;
        }

        return grid[y][x] != '.';
    }

    private char[][] Parse(bool useExample)
    {
        return ParseLines<char[]>(useExample ? "example.txt" : "input.txt", text => text.ToCharArray()).ToArray();
    }
}
