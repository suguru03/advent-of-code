using System.Text.RegularExpressions;
using advent_of_code.Common;
using advent_of_code.Common.utils;

namespace advent_of_code._2023.day18;

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

    private static int Solve1(Data data)
    {
        var groupId = 1;
        var grid = new Dictionary<Vector2D, int>();
        var neighborSet = new HashSet<Vector2D>();
        var start = Vector2D.Zero;
        var pos = start;
        var horizontal = new Range<int>(0, 0);
        var vertical = new Range<int>(0, 0);
        foreach (var (delta, size) in data.List)
        {
            for (var i = 0; i < size; i++)
            {
                pos += delta;
                grid[pos] = groupId;
                foreach (var neighbor in pos.GetAdjacentSet())
                {
                    neighborSet.Add(neighbor);
                }
            }

            horizontal = new Range<int>(Math.Min(horizontal.Min, pos.X), Math.Max(horizontal.Max, pos.X));
            vertical = new Range<int>(Math.Min(vertical.Min, pos.Y), Math.Max(vertical.Max, pos.Y));
        }

        var interiorGroupSet = new HashSet<int> { groupId };
        foreach (var neighbor in neighborSet)
        {
            if (grid.ContainsKey(neighbor))
            {
                continue;
            }

            if (!CheckInteriorAndFillGroup(grid, horizontal, vertical, neighbor, ++groupId, interiorGroupSet))
            {
                continue;
            }

            interiorGroupSet.Add(groupId);
        }

        return CountTrenches(grid, interiorGroupSet);
    }

    private static bool CheckInteriorAndFillGroup(IDictionary<Vector2D, int> grid, Range<int> horizontal,
        Range<int> vertical, Vector2D neighbor, int groupId,
        ISet<int> interiorGroupSet)
    {
        var queue = new Queue<Vector2D>();
        queue.Enqueue(neighbor);
        while (queue.Count != 0)
        {
            var pos = queue.Dequeue();
            if (!horizontal.Contains(pos.X) || !vertical.Contains(pos.Y))
            {
                return false;
            }

            if (grid.TryGetValue(pos, out var gid))
            {
                if (gid == groupId)
                {
                    continue;
                }

                if (!interiorGroupSet.Contains(gid))
                {
                    return false;
                }

                continue;
            }

            grid[pos] = groupId;
            foreach (var next in pos.GetAdjacentSet())
            {
                queue.Enqueue(next);
            }
        }

        return true;
    }

    private static int CountTrenches(Dictionary<Vector2D, int> grid, ISet<int> interiorGroupSet)
    {
        return grid.Values.Sum(groupId => Convert.ToInt32(interiorGroupSet.Contains(groupId)));
    }

    private static readonly Dictionary<string, Vector2D> DeltaMap = new()
    {
        { "U", Vector2D.Up },
        { "D", Vector2D.Down },
        { "L", Vector2D.Left },
        { "R", Vector2D.Right }
    };

    private Data Parse(bool useExample)
    {
        return new Data(ParseLines(useExample ? "example.txt" : "input.txt", text =>
        {
            var matches = Regex.Match(text, @"(.+) (\d+) \((.+)\)").Groups.Values.ToList();
            var delta = DeltaMap[matches[1].Value];
            var size = int.Parse(matches[2].Value);
            return new Instruction(delta, size);
        }).ToList());
    }

    private record Data(List<Instruction> List);

    private record Instruction(Vector2D Delta, int Size);
}
