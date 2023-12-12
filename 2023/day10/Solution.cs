using advent_of_code.Common;

namespace advent_of_code._2023.day10;

public class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var input = Parse(part, useExample);
        return part switch
        {
            1 => Solve1(input),
            2 => Solve2(input),
            _ => ProblemNotSolvedString
        };
    }


    private const char Unknown = '?';
    private const char Ground = '.';
    private const char Wall = '■';
    private const char InLoop = 'I';
    private const char OutOfLoop = 'O';

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
                if (!ValidPosition(grid, cur))
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

                Fill(grid, cur, Ground);
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

    private static int Solve2(IReadOnlyList<char[]> data)
    {
        var grid = string.Join($"\n{new string(Unknown, data[0].Length * 2 - 1)}\n",
            data.Select(list => string.Join(Unknown, list))).Split("\n").Select(row => row.ToCharArray()).ToList();
        var queue = new Queue<((int x, int y) from, (int x, int y) current)>();
        queue.Enqueue(((-1, -1), FindStart(grid)));
        while (queue.Count > 0)
        {
            var (from, cur) = queue.Dequeue();
            if (!ValidPosition(grid, cur))
            {
                continue;
            }

            switch (grid[cur.y][cur.x])
            {
                case 'S':
                {
                    queue.Enqueue((cur, (cur.x - 2, cur.y)));
                    queue.Enqueue((cur, (cur.x + 2, cur.y)));
                    queue.Enqueue((cur, (cur.x, cur.y - 2)));
                    queue.Enqueue((cur, (cur.x, cur.y + 2)));
                    Fill(grid, cur, Wall);
                    continue;
                }
                case '|':
                {
                    if (cur.x != from.x)
                    {
                        continue;
                    }

                    queue.Enqueue((cur, (cur.x, cur.y - 2)));
                    queue.Enqueue((cur, (cur.x, cur.y + 2)));
                    break;
                }
                case '-':
                {
                    if (cur.y != from.y)
                    {
                        continue;
                    }

                    queue.Enqueue((cur, (cur.x - 2, cur.y)));
                    queue.Enqueue((cur, (cur.x + 2, cur.y)));
                    break;
                }
                case 'L':
                {
                    if (cur.x + 2 != from.x && cur.y - 2 != from.y)
                    {
                        continue;
                    }

                    queue.Enqueue((cur, (cur.x + 2, cur.y)));
                    queue.Enqueue((cur, (cur.x, cur.y - 2)));
                    break;
                }
                case 'J':
                {
                    if (cur.x - 2 != from.x && cur.y - 2 != from.y)
                    {
                        continue;
                    }

                    queue.Enqueue((cur, (cur.x - 2, cur.y)));
                    queue.Enqueue((cur, (cur.x, cur.y - 2)));
                    break;
                }
                case '7':
                {
                    if (cur.x - 2 != from.x && cur.y + 2 != from.y)
                    {
                        continue;
                    }

                    queue.Enqueue((cur, (cur.x - 2, cur.y)));
                    queue.Enqueue((cur, (cur.x, cur.y + 2)));
                    break;
                }
                case 'F':
                {
                    if (cur.x + 2 != from.x && cur.y + 2 != from.y)
                    {
                        continue;
                    }

                    queue.Enqueue((cur, (cur.x + 2, cur.y)));
                    queue.Enqueue((cur, (cur.x, cur.y + 2)));
                    break;
                }
            }

            Fill(grid, cur, Wall);
            FillMid(grid, from, cur, Wall);
        }

        var count = 0;
        for (var y = 0; y < grid.Count; y += 2)
        for (var x = 0; x < grid[y].Length; x += 2)
        {
            switch (grid[y][x])
            {
                case Wall:
                case InLoop:
                case OutOfLoop:
                {
                    continue;
                }
            }

            var cur = (x, y);
            var isInLoop = IsInLoop(grid, new HashSet<(int x, int y)>(), cur);
            var mark = isInLoop ? InLoop : OutOfLoop;
            FillGrid(grid, cur, mark);
            grid[y][x] = mark;
            count += Convert.ToInt32(isInLoop);
        }

        return count;
    }

    private static (int, int) FindStart(IReadOnlyList<char[]> grid)
    {
        for (var y = 0; y < grid.Count; y++)
        for (var x = 0; x < grid[y].Length; x++)
        {
            if (grid[y][x] == 'S')
            {
                return (x, y);
            }
        }

        return (-1, -1);
    }

    private static bool ValidPosition(IReadOnlyList<char[]> grid, (int x, int y) cur)
    {
        var (x, y) = cur;
        return y >= 0 && y < grid.Count && x >= 0 && x < grid[y].Length;
    }

    private static bool IsInLoop(IReadOnlyList<char[]> grid, ISet<(int x, int y)> seen, (int x, int y) cur)
    {
        if (!ValidPosition(grid, cur))
        {
            return false;
        }

        if (seen.Contains(cur))
        {
            return true;
        }

        seen.Add(cur);
        var (x, y) = cur;
        switch (grid[y][x])
        {
            case Wall:
            case InLoop:
            {
                return true;
            }
            case OutOfLoop:
            {
                return false;
            }
        }

        return IsInLoop(grid, seen, (x - 1, y)) &&
               IsInLoop(grid, seen, (x + 1, y)) &&
               IsInLoop(grid, seen, (x, y - 1)) &&
               IsInLoop(grid, seen, (x, y + 1));
    }

    private static void FillGrid(IReadOnlyList<char[]> grid, (int x, int y) cur, char c)
    {
        if (!ValidPosition(grid, cur))
        {
            return;
        }

        var (x, y) = cur;
        switch (grid[y][x])
        {
            case Wall:
            case InLoop:
            case OutOfLoop:
            {
                return;
            }
            case Unknown:
            {
                grid[y][x] = c;
                break;
            }
        }

        FillGrid(grid, (x - 1, y), c);
        FillGrid(grid, (x + 1, y), c);
        FillGrid(grid, (x, y - 1), c);
        FillGrid(grid, (x, y + 1), c);
    }

    private static void Fill(IReadOnlyList<char[]> grid, int x, int y, char c)
    {
        grid[y][x] = c;
    }

    private static void Fill(IReadOnlyList<char[]> grid, (int x, int y) t, char c)
    {
        Fill(grid, t.x, t.y, c);
    }

    private static void FillMid(IReadOnlyList<char[]> grid, (int x, int y) from, (int x, int y) to, char c)
    {
        Fill(grid, from.x + (to.x - from.x) / 2, from.y + (to.y - from.y) / 2, c);
    }

    private char[][] Parse(int part, bool useExample)
    {
        return ParseLines<char[]>(useExample ? $"example{part}.txt" : "input.txt", text => text.ToCharArray())
            .ToArray();
    }
}
