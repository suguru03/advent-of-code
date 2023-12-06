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
        foreach (var maps in data.MapsList)
        {
            maps.Sort((m1, m2) => (int)(m1.From - m2.From));
        }

        var min = long.MaxValue;
        foreach (var seed in data.Seeds)
        {
            var cur = seed;
            foreach (var maps in data.MapsList)
            {
                Console.WriteLine($"--");
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

                Console.WriteLine($"cur: {cur}");
            }

            Console.WriteLine($"seed: {seed}, num: {cur}");
            min = Math.Min(min, cur);
        }

        return min;
    }

    private static long Solve2(Data data)
    {
        // var seeds2 = data.Seeds.ToList();
        // var nextSeeds = new List<long>();
        // var nextSeeds = new List<long> { 24799538, 1636419363 };
        // for (var i = 0; i < seeds2.Count / 2; i += 2)
        // {
        //     for (var j = seeds2[i]; j < seeds2[i] + seeds2[i + 1]; j++)
        //     {
        //         nextSeeds.Add(j);
        //     }
        // }

        // var nextSeeds = new List<long> { 24799538 };
        // return Solve1(new Data(nextSeeds, data.MapsList));

        foreach (var maps in data.MapsList)
        {
            maps.Sort((m1, m2) => (int)(m1.Destination - m2.Destination));
        }

        // var target = -1;
        var target = 3274577479;
        var mapsList = new List<List<MapData>>();
        var maxValue = data.MapsList.SelectMany(item => item).Select(item => item.Destination + item.Range).Max();
        foreach (var maps in data.MapsList)
        {
            var left = 0L;
            var nextMaps = new List<MapData>();
            mapsList.Add(nextMaps);
            foreach (var map in maps)
            {
                if (target != -1 && mapsList.Count == 7)
                {
                    if (target < map.Destination)
                    {
                        nextMaps.Add(new MapData(target, target, 1));
                        break;
                    }

                    if (target >= map.Destination && target < map.Destination + map.Range)
                    {
                        nextMaps.Add(new MapData(target, target + (map.From - map.Destination), 1));
                        break;
                    }

                    continue;
                }

                if (left < map.Destination)
                {
                    nextMaps.Add(new MapData(left, left, map.Destination - left));
                }

                nextMaps.Add(map);
                left = map.Destination + map.Range;
            }

            if (target != -1 && mapsList.Count == 7)
            {
                continue;
            }

            nextMaps.Add(new MapData(left, left, maxValue));
        }

        foreach (var maps in mapsList)
        {
            Console.WriteLine($"--");
            foreach (var map in maps)
            {
                Console.WriteLine($"{map.Destination}, {map.From}, {map.Range}");
            }
        }

        var seeds = data.Seeds.ToList();
        var seedRanges = new List<SeedRange>();
        for (var i = 0; i < seeds.Count / 2; i += 2)
        {
            seedRanges.Add(new SeedRange(seeds[i], seeds[i] + seeds[i + 1]));
        }

        // 3274577479 too high
        var (minSeed, _, resultList) = FindMin(mapsList.Count, new MapData(1, 0, maxValue), mapsList, seedRanges);
        return Solve1(new Data(new List<long> { minSeed }, data.MapsList));
    }

    private static (long, long, List<List<MapData>>) FindMin(int mapsIndex, MapData prev, IList<List<MapData>> mapsList,
        List<SeedRange> seedRanges)
    {
        // find seeds
        if (mapsIndex == 0)
        {
            foreach (var seed in seedRanges)
            {
                if (seed.From >= prev.From + prev.Range || seed.To < prev.From)
                {
                    continue;
                }

                var offset = Math.Max(0, seed.From - prev.From);
                var result = prev.From + offset; // 82
                return (result, offset, new List<List<MapData>>());
            }

            return (-1, -1, null);
        }


        // { 0, 0, 56 }
        Console.WriteLine($"-- {mapsIndex - 1} --");
        Console.WriteLine($"prev: {prev.Destination}, {prev.From}, {prev.Range}");

        var maps = mapsList[--mapsIndex];
        foreach (var map in maps)
        {
            var left = map.Destination;
            var right = left + map.Range;
            if (left >= prev.From + prev.Range || right <= prev.From)
            {
                continue;
            }


            // 0, 69, 1 - 1, 0, 69
            var nextDestination = Math.Max(prev.From, left); // 0 - 1
            var nextRight = Math.Min(prev.From + prev.Range, right); // 1 - 56
            var offset = map.From - map.Destination; // 69 - -1
            var nextFrom = nextDestination + offset; // 69 - 0
            var nextRange = nextRight - nextDestination; // 1 - 55
            Console.WriteLine($"current: {map.Destination}, {map.From}, {map.Range}");
            Console.WriteLine($"next: {nextDestination}, {nextFrom}, {nextRange}");
            // 0, 69, 1 - 1, 0, 55
            var next = new MapData(nextDestination, nextFrom, nextRange);
            var (result, seedOffset, list) = FindMin(mapsIndex, new MapData(nextDestination, nextFrom, nextRange),
                mapsList,
                seedRanges);
            if (result != -1)
            {
                list.Add(new List<MapData> { next });
                return (result, seedOffset, list);
            }
        }

        return (-1, -1, null);
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
            return new Data(seeds, mapsList);
        });
    }

    private class Data
    {
        public IEnumerable<long> Seeds { get; }
        public IEnumerable<List<MapData>> MapsList { get; }

        public Data(IEnumerable<long> seeds, IEnumerable<List<MapData>> mapsList)
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
