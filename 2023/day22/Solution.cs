using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2023.day22;

public partial class Solution : SolutionBase
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

    private static int Solve1(IEnumerable<Cube> snapshot)
    {
        return Settle(snapshot).Count(cube => cube.Disintegrable);
    }

    private static int Solve2(IEnumerable<Cube> snapshot)
    {
        return Settle(snapshot).Skip(1).Where(cube => !cube.Disintegrable).Sum(CountSupporting);
    }

    private static int CountSupporting(Cube root)
    {
        var supportingMap = new Dictionary<Cube, bool>();
        var stack = new Stack<Cube>(collection: [root]);
        while (stack.Count != 0)
        {
            var cube = stack.Pop();
            foreach (var next in cube.Supporting)
            {
                if (supportingMap.ContainsKey(next))
                {
                    continue;
                }

                var supported = next.IsSupported(root, supportingMap);
                supportingMap.Add(next, !supported);
                if (supported)
                {
                    continue;
                }

                stack.Push(next);
            }
        }

        return supportingMap.Count(kvp => kvp.Value);
    }

    private static IEnumerable<Cube> Settle(IEnumerable<Cube> snapshot)
    {
        var cubes = snapshot.ToList();
        cubes.Add(new Cube(new Coordinate(0, 0, 0), new Coordinate(int.MaxValue, int.MaxValue, 0)));
        cubes.Sort((c1, c2) => c1.From.Z.CompareTo(c2.From.Z));
        for (var i = 1; i < cubes.Count; i++)
        {
            var target = cubes[i];
            var candidates = new List<Cube>();
            for (var j = i - 1; j >= 0; j--)
            {
                var supporter = cubes[j];
                if (target.From.Z <= supporter.To.Z ||
                    target.From.X > supporter.To.X || target.To.X < supporter.From.X ||
                    target.From.Y > supporter.To.Y || target.To.Y < supporter.From.Y)
                {
                    continue;
                }


                if (candidates.Count == 0)
                {
                    candidates.Add(supporter);
                    continue;
                }

                var first = candidates.First();
                if (supporter.To.Z < first.To.Z)
                {
                    continue;
                }

                if (supporter.To.Z > candidates.First().To.Z)
                {
                    candidates.Clear();
                }

                candidates.Add(supporter);
            }

            if (candidates.Count == 0)
            {
                continue;
            }

            var shift = target.From.Z - candidates.First().To.Z - 1;
            target.From.Z -= shift;
            target.To.Z -= shift;
            foreach (var supporter in candidates)
            {
                supporter.Support(target);
            }
        }

        return cubes;
    }

    private IEnumerable<Cube> Parse(bool useExample)
    {
        var cubeRegex = CubeRegex();
        return ParseLines(useExample ? "example.txt" : "input.txt", text =>
        {
            var groups = cubeRegex.Match(text).Groups;
            return new Cube(ParseCoordinate(groups["From"].Value), ParseCoordinate(groups["To"].Value));
        }).ToList();
    }

    private static Coordinate ParseCoordinate(string input)
    {
        var nums = input.Split(',').Select(str => int.Parse(str.Trim())).ToList();
        return new Coordinate(nums[0], nums[1], nums[2]);
    }

    private class Coordinate(int x, int y, int z)
    {
        public int X => x;
        public int Y => y;
        public int Z { get; set; } = z;
    }

    private record Cube(Coordinate From, Coordinate To)
    {
        private static readonly Dictionary<Cube, bool> Empty = new();
        public readonly HashSet<Cube> Supporting = [];
        private readonly HashSet<Cube> _supported = [];
        public bool Disintegrable => Supporting.All(cube => cube.IsSupported(this, Empty));

        public void Support(Cube cube)
        {
            Supporting.Add(cube);
            cube._supported.Add(this);
        }

        public bool IsSupported(Cube excluded, IDictionary<Cube, bool> supportingByExcludedMap)
        {
            return From.Z <= excluded.To.Z || (supportingByExcludedMap.TryGetValue(this, out var supporting)
                ? !supporting
                : _supported.Any(cube => cube != excluded && cube.IsSupported(excluded, supportingByExcludedMap)));
        }
    }

    [GeneratedRegex(@"(?<From>-?.+)\~(?<To>-?.+)")]
    private static partial Regex CubeRegex();
}
