using advent_of_code.Common;

namespace advent_of_code._2023.day14;

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
        var grid = data.Grid;
        var sum = 0;
        var top = 0;
        var depth = grid[0].Count;
        for (var x = 0; x < depth; x++, top = 0)
        for (var y = 0; y < grid.Count; y++)
        {
            switch (grid[y][x])
            {
                case 'O':
                {
                    sum += depth - top++;
                    break;
                }
                case '#':
                {
                    top = y + 1;
                    break;
                }
                case '.':
                {
                    continue;
                }
            }
        }

        return sum;
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseLines(useExample ? "example.txt" : "input.txt", text => text.ToList()).ToList());
    }

    private readonly record struct Data(List<List<char>> Grid);
}
