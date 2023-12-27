using System.Text.RegularExpressions;
using advent_of_code.Common;
using advent_of_code.Common.utils;

namespace advent_of_code._2023.day19;

public partial class Solution : SolutionBase
{
    private const string Start = "in";
    private const string Accepted = "A";
    private const string Rejected = "R";
    private static readonly Range<int> RatingRange = new(1, 4000);

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

    private static int Solve1(Data data)
    {
        var (workflowMap, ratingMaps) = data;
        return ratingMaps.Where(ratingMap => IsAccepted(workflowMap, ratingMap))
            .Sum(ratingMap => ratingMap.Values.Sum());
    }

    private static bool IsAccepted(IReadOnlyDictionary<string, Workflow> workflowMap,
        IReadOnlyDictionary<char, int> ratingMap)
    {
        var current = Start;
        while (current != Accepted && current != Rejected)
        {
            current = workflowMap[current].Expressions.Find(expression => expression.If(ratingMap))!.Consequent;
        }

        return current == Accepted;
    }

    private static long Solve2(Data data)
    {
        var workflowMap = new Dictionary<Expression, Workflow>();
        var expressionsMap = new Dictionary<string, List<Expression>>();
        foreach (var workflow in data.WorkflowMap.Values)
        foreach (var expression in workflow.Expressions)
        {
            workflowMap.Add(expression, workflow);
            if (!expressionsMap.TryGetValue(expression.Consequent, out var list))
            {
                list = new List<Expression>();
                expressionsMap.Add(expression.Consequent, list);
            }

            list.Add(expression);
        }

        var chars = data.RatingMaps.First().Keys;
        return expressionsMap[Accepted].Sum(expression =>
            SumCombinations(expression, workflowMap, expressionsMap, chars.ToDictionary(c => c, _ => RatingRange)));
    }

    private static long SumCombinations(Expression current, IReadOnlyDictionary<Expression, Workflow> workflowMap,
        IReadOnlyDictionary<string, List<Expression>> expressionsMap,
        Dictionary<char, Range<int>> rangeMap)
    {
        UpdateRange(current.Condition, rangeMap, false);
        var (workflowId, expressions) = workflowMap[current];
        foreach (var prevExpression in expressions)
        {
            if (prevExpression == current)
            {
                break;
            }

            UpdateRange(prevExpression.Condition, rangeMap, true);
        }

        return workflowId == Start
            ? rangeMap.Values.Aggregate(1L, (acc, range) => acc * (range.Max - range.Min + 1))
            : expressionsMap[workflowId].Sum(next => SumCombinations(next, workflowMap, expressionsMap, rangeMap));
    }

    private static void UpdateRange(Condition? condition, IDictionary<char, Range<int>> rangeMap, bool reverse)
    {
        if (condition == null)
        {
            return;
        }

        rangeMap[condition.Category] = GetAndRange(rangeMap[condition.Category], condition.GetRange(reverse));
    }

    private static Range<int> GetAndRange(Range<int> lhs, Range<int> rhs)
    {
        return new Range<int>(Math.Max(lhs.Min, rhs.Min), Math.Min(lhs.Max, rhs.Max));
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

    private record Condition(char Category, char Operator, int Value)
    {
        public bool Check(int value)
        {
            return Operator == '>' ? value > Value : value < Value;
        }

        public Range<int> GetRange(bool reverse = false)
        {
            var greater = Operator == '>';
            return reverse
                ? greater
                    ? RatingRange with { Max = Value }
                    : RatingRange with { Min = Value }
                : greater
                    ? RatingRange with { Min = Value + 1 }
                    : RatingRange with { Max = Value - 1 };
        }
    }

    private class Expression
    {
        public string Consequent { get; }
        public Condition? Condition { get; }

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
            return Condition?.Check(ratingMap[Condition.Category]) ?? true;
        }
    }

    [GeneratedRegex("(.+){(.+)}")]
    private static partial Regex WorkflowRegex();

    [GeneratedRegex(@"{?(.+)=(\d+)}?")]
    private static partial Regex RatingRegex();

    [GeneratedRegex(@"(.)([<>])(\d+):(.+)")]
    private static partial Regex ExpressionRegex();
}
