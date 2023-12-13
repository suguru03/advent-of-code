using System.Diagnostics;
using advent_of_code.Common;

namespace advent_of_code._2023.day12;

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

    public override void Test()
    {
        var testCases = new List<(string, string, int)>
        {
            ("???.###", "1,1,3", 1),
            (".??..??...?##.", "1,1,3", 4),
            ("?#?#?#?#?#?#?#?", "1,3,1,6", 1),
            ("?###????????", "3,2,1", 10),
            (".??.?#?????", "2,2", 7),
            ("?#??#??", "2,1", 2),
        };
        foreach (var (records, sizes, expected) in testCases)
        {
            var result = Solve1(new List<Data>
                { new($"{records}.".ToCharArray(), sizes.Split(',').Select(int.Parse).ToList()) });
            var message = $"{records} {sizes} -> {expected}";
            Debug.Assert(result == expected, $"{message} != {result}");
            Console.WriteLine($"[PASSED] {message}");
        }
    }


    private static long Solve1(IEnumerable<Data> data)
    {
        return Sum(data);
    }

    private static long Solve2(IEnumerable<Data> data)
    {
        return Sum(data);
    }

    private enum State
    {
        None,
        FoundDamaged,
        End,
    }

    private static long Sum(IEnumerable<Data> data)
    {
        return data.Select(row => Sum(row.Records, 0, row.Sizes, 0, new Dictionary<(int, int), long>())).Sum();
    }

    private static long Sum(IList<char> records, int rIndex, IList<int> sizes, int sIndex,
        IDictionary<(int, int), long> cacheMap)
    {
        if (rIndex == records.Count || sIndex == sizes.Count)
        {
            return 0;
        }

        var cacheKey = (rIndex, sIndex);
        if (cacheMap.TryGetValue(cacheKey, out var cache))
        {
            return cache;
        }

        var sum = 0L;
        var size = sizes[sIndex];
        var left = rIndex;
        var right = left;
        var state = State.None;
        while (right < records.Count - 1 && state != State.End)
        {
            switch (records[right++])
            {
                case '.':
                {
                    if (state == State.FoundDamaged)
                    {
                        state = State.End;
                    }
                    else
                    {
                        left = right;
                    }

                    continue;
                }
                case '#':
                {
                    state = State.FoundDamaged;
                    break;
                }
                case '?':
                {
                    break;
                }
            }

            if (right - left != size)
            {
                continue;
            }

            if (records[left++] == '#')
            {
                state = State.End;
            }

            if (records[right] == '#')
            {
                continue;
            }

            sum += sizes.Count - sIndex == 1
                ? Convert.ToInt32(!records.Skip(right).Contains('#'))
                : Sum(records, right + 1, sizes, sIndex + 1, cacheMap);
        }

        cacheMap.Add(cacheKey, sum);
        return sum;
    }

    private IEnumerable<Data> Parse(int part, bool useExample)
    {
        return ParseLines(useExample ? "example.txt" : "input.txt", text =>
        {
            var items = text.Trim().Split(' ')
                .Select((row, i) => string.Join(i == 0 ? '?' : ',', Enumerable.Repeat(row, part == 1 ? 1 : 5)))
                .ToList();
            return new Data($"{items[0]}.".ToCharArray(), items[1].Split(',').Select(int.Parse).ToList());
        });
    }

    private readonly record struct Data(IList<char> Records, IList<int> Sizes);
}
