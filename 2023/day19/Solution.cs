using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2023.day19;

public partial class Solution : SolutionBase
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
        var (workflowMap, ratingMaps) = data;
        return ratingMaps.Where(ratingMap => IsAccepted(workflowMap, ratingMap))
            .Sum(ratingMap => ratingMap.Values.Sum());
    }

    private static bool IsAccepted(IReadOnlyDictionary<string, Workflow> workflowMap,
        IReadOnlyDictionary<char, int> ratingMap)
    {
        const string start = "in";
        const string accepted = "A";
        const string rejected = "R";
        var current = start;
        while (current != accepted && current != rejected)
        {
            current = workflowMap[current].Expressions.Find(expression => expression.If(ratingMap))!.Consequent;
        }

        return current == accepted;
    }

    private Data Parse(bool useExample)
    {
        var rows = ParseLines(useExample ? "example.txt" : "input.txt", text => text).ToList();
        var index = rows.FindIndex(string.IsNullOrWhiteSpace);

        var workflowRegex = WorkflowRegex();
        var workflowMap = rows.Take(index).Select(text =>
        {
            var groups = workflowRegex.Match(text).Groups.Values.ToList();
            return new Workflow(groups[1].Value,
                groups[2].Value.Split(',').Select(statement => new Expression(statement)).ToList());
        }).ToDictionary(row => row.Id);

        var ratingRegex = RatingRegex();
        var ratingMaps = rows.Skip(index + 1).Select(text => text.Split(',').Select(statement =>
        {
            var groups = ratingRegex.Match(statement).Groups.Values.ToList();
            return new KeyValuePair<char, int>(groups[1].Value.First(), int.Parse(groups[2].Value));
        }).ToDictionary());

        return new Data(workflowMap, ratingMaps);
    }

    private record Data(Dictionary<string, Workflow> WorkflowMap, IEnumerable<Dictionary<char, int>> RatingMaps);

    private record Workflow(string Id, List<Expression> Expressions);

    private record Condition(char Category, char Operator, int Value);

    private class Expression
    {
        public string Consequent { get; }
        private Condition? Condition { get; }

        public Expression(string text)
        {
            var groups = ExpressionRegex().Match(text).Groups.Values.ToList();
            if (groups.Count == 1)
            {
                Consequent = text;
                return;
            }

            Condition = new Condition(groups[1].Value.First(), groups[2].Value.First(), int.Parse(groups[3].Value));
            Consequent = groups[4].Value;
        }

        public bool If(IReadOnlyDictionary<char, int> ratingMap)
        {
            if (Condition == null)
            {
                return true;
            }

            var value = ratingMap[Condition.Category];
            return Condition?.Operator switch
            {
                '<' => value < Condition.Value,
                '>' => value > Condition.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    [GeneratedRegex("(.+){(.+)}")]
    private static partial Regex WorkflowRegex();

    [GeneratedRegex(@"{?(.+)=(\d+)}?")]
    private static partial Regex RatingRegex();

    [GeneratedRegex(@"(.)([<>])(\d+):(.+)")]
    private static partial Regex ExpressionRegex();
}
