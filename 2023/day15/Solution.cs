using advent_of_code.Common;

namespace advent_of_code._2023.day15;

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
        const int multiplier = 17;
        const int divisor = 256;
        return data.List.Select(str => str.Aggregate(0, (sum, c) => (sum + c) * multiplier % divisor)).Sum();
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseText(useExample ? "example.txt" : "input.txt", text => text.Trim().Split(',')));
    }

    private readonly record struct Data(IEnumerable<string> List);
}
