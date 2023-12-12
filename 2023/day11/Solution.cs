using advent_of_code.Common;

namespace advent_of_code._2023.day11;

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

    private static int Solve1(IEnumerable<IEnumerable<char>> data)
    {
        var grid = data.Select(row => row.ToList()).ToList();
        var galaxies = new List<(int x, int y)>();
        var xSet = new HashSet<int>();
        var ySet = new HashSet<int>();
        for (var y = 0; y < grid.Count; y++)
        {
            for (var x = 0; x < grid[y].Count; x++)
            {
                if (grid[y][x] != '#')
                {
                    continue;
                }

                galaxies.Add((x, y));
                xSet.Add(x);
                ySet.Add(y);
            }
        }

        return Sum(0);

        int Sum(int left)
        {
            if (left == galaxies.Count - 1)
            {
                return 0;
            }

            var sum = 0;
            var right = left;
            while (++right < galaxies.Count)
            {
                var gl = galaxies[left];
                var gr = galaxies[right];
                sum += Diff(gl.x, gr.x, xSet) + Diff(gl.y, gr.y, ySet);
            }

            return sum + Sum(left + 1);

            int Diff(int l, int y, ICollection<int> set)
            {
                var d = Math.Sign(y - l);
                var diff = 0;
                while (l != y)
                {
                    l += d;
                    diff++;
                    diff += Convert.ToInt32(!set.Contains(l));
                }

                return diff;
            }
        }
    }


    private IEnumerable<IEnumerable<char>> Parse(bool useExample)
    {
        return ParseLines<IEnumerable<char>>(useExample ? "example.txt" : "input.txt",
            text => text.ToCharArray());
    }
}
