using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2023.day5;

public class Solution : SolutionBase
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

    private static long Solve1(Data data)
    {
        var min = long.MaxValue;
        foreach (var seed in data.Seeds)
        {
            var cur = seed;
            foreach (var maps in data.MapsList)
            {
                foreach (var map in maps)
                {
                    var left = map.From;
                    var right = left + map.Range;
                    if (cur < left || cur >= right)
                    {
                        continue;
                    }

                    cur += map.Destination - left;
                    break;
                }
            }

            min = Math.Min(min, cur);
        }

        return min;
    }
    private Data Parse(bool useExample)
    {
        return ParseText<Data>(useExample ? "example.txt" : "input.txt", row =>
        {
            var rows = row.Split('\n');
            var seeds = Regex.Match(rows[0], "seeds: (.+)").Groups[1].Value.Trim().Split(" ").Select(long.Parse);
            var mapsList = rows.ToList().GetRange(2, rows.Length - 2).Aggregate(new List<List<MapData>>(),
                (list, mapData) =>
                {
                    if (Regex.IsMatch(mapData, "(.+):"))
                    {
                        list.Add(new List<MapData>());
                        return list;
                    }

                    var nums = mapData.Split(" ").Select(data => data.Trim()).Where(data => !string.IsNullOrEmpty(data))
                        .Select(long.Parse).ToList();
                    if (nums.Count == 3)
                    {
                        list.Last().Add(new MapData(nums[0], nums[1], nums[2]));
                    }

                    return list;
                });
            foreach (var maps in mapsList)
            {
                maps.Sort((m1, m2) => (int)(m1.From - m2.From));
            }

            return new Data(seeds, mapsList);
        });
    }

    private class Data
    {
        public IEnumerable<long> Seeds { get; }
        public IEnumerable<IEnumerable<MapData>> MapsList { get; }

        public Data(IEnumerable<long> seeds, IEnumerable<IEnumerable<MapData>> mapsList)
        {
            Seeds = seeds;
            MapsList = mapsList;
        }
    }

    private class MapData
    {
        public long Destination { get; }
        public long From { get; }
        public long Range { get; }

        public MapData(long destination, long from, long range)
        {
            Destination = destination;
            From = from;
            Range = range;
        }
    }

    private class SeedRange
    {
        public long From { get; }
        public long To { get; }

        public SeedRange(long from, long to)
        {
            From = from;
            To = to;
        }
    }
}
