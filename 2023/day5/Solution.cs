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
            maps.Sort((m1, m2) => m1.Source.CompareTo(m2.Source));
        }

        var min = long.MaxValue;
        foreach (var seed in data.Seeds)
        {
            var cur = seed;
            foreach (var maps in data.MapsList)
            {
                foreach (var (destination, source, range) in maps)
                {
                    var limit = source + range;
                    if (cur < source || cur >= limit)
                    {
                        continue;
                    }

                    cur += destination - source;
                    break;
                }
            }

            min = Math.Min(min, cur);
        }

        return min;
    }

    private static long Solve2(Data data)
    {
        var seeds = data.Seeds.ToList();
        var seedRanges = seeds.Chunk(2).Select(row => new SeedRange(row.First(), row.First() + row.Last())).ToList();
        foreach (var maps in data.MapsList)
        {
            maps.Sort((m1, m2) => m1.Destination.CompareTo(m2.Destination));
        }

        var mapsList = new List<List<MapData>>();
        var maxValue = data.MapsList.SelectMany(item => item).Select(item => item.Destination + item.Range).Max();
        foreach (var maps in data.MapsList)
        {
            var left = 0L;
            var nextMaps = new List<MapData>();
            mapsList.Add(nextMaps);
            foreach (var map in maps)
            {
                if (left < map.Destination)
                {
                    nextMaps.Add(new MapData(left, left, map.Destination - left));
                }

                nextMaps.Add(map);
                left = map.Destination + map.Range;
            }

            nextMaps.Add(new MapData(left, left, maxValue));
        }

        var minSeed = FindMin(mapsList.Count, new MapData(0, 0, maxValue), mapsList, seedRanges);
        return Solve1(data with { Seeds = new List<long> { minSeed } });
    }

    private static long FindMin(int mapsIndex, MapData prev, IList<List<MapData>> mapsList, List<SeedRange> seedRanges)
    {
        // find seeds
        if (mapsIndex == 0)
        {
            var seed = seedRanges.Find(seed => seed.From < prev.Source + prev.Range && seed.To > prev.Source);
            return seed != null ? Math.Max(prev.Source, seed.From) : -1;
        }

        var maps = mapsList[--mapsIndex];
        foreach (var (destination, source, range) in maps)
        {
            var limit = destination + range;
            if (destination >= prev.Source + prev.Range || limit <= prev.Source)
            {
                continue;
            }

            var nextDestination = Math.Max(prev.Source, destination);
            var nextRight = Math.Min(prev.Source + prev.Range, limit);
            var offset = source - destination;
            var nextSource = nextDestination + offset;
            var nextRange = nextRight - nextDestination;
            var result = FindMin(mapsIndex, new MapData(nextDestination, nextSource, nextRange), mapsList, seedRanges);
            if (result == -1)
            {
                continue;
            }

            return result;
        }

        return -1;
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
                        list.Add([]);
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

    private record Data(IEnumerable<long> Seeds, IEnumerable<List<MapData>> MapsList);

    private record MapData(long Destination, long Source, long Range);

    private record SeedRange(long From, long To);
}
