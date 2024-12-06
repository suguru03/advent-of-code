using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2024.day5;

public partial class Solution : SolutionBase
{
    public override object Run(int part, bool useExample)
    {
        var rules = new List<(int x, int y)>();
        var updatesList = new List<int[]>();
        var rows = ParseLines(!useExample ? "input.txt" : "example.txt", row => row);
        var regex = RuleRegex();
        foreach (var row in rows)
        {
            var match = regex.Match(row);
            if (match.Success)
            {
                rules.Add((int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)));
                continue;
            }

            if (string.IsNullOrWhiteSpace(row))
            {
                continue;
            }

            var updates = row.Split(',').Select(int.Parse).ToArray();
            updatesList.Add(updates);
        }

        return part switch
        {
            1 => Solve1(rules, updatesList),
            _ => ProblemNotSolvedString
        };
    }

    private static int Solve1(IList<(int x, int y)> rules, IList<int[]> updatesList)
    {
        var rulesMap = ConstructRulesMap(rules);
        return updatesList.Where(updates => IsValid(updates, rulesMap))
            .Select(updates => updates[updates.Length / 2])
            .Sum();
    }

    private static Dictionary<int, HashSet<int>> ConstructRulesMap(IList<(int x, int y)> rules)
    {
        var rulesMap = new Dictionary<int, HashSet<int>>();
        foreach (var (x, y) in rules)
        {
            if (!rulesMap.TryGetValue(x, out var set))
            {
                set = [];
                rulesMap.Add(x, set);
            }

            set.Add(y);
        }

        return rulesMap;
    }

    private static bool IsValid(int[] updates, Dictionary<int, HashSet<int>> rulesMap)
    {
        return updates.Take(updates.Length - 1)
            .Select((update, index) => (update, index))
            .All(pair =>
                rulesMap.TryGetValue(pair.update, out var set) &&
                updates.Skip(pair.index + 1).All(update => set.Contains(update))
            );
    }

    [GeneratedRegex(@"(\d+)\|(\d+)")]
    private static partial Regex RuleRegex();
}
