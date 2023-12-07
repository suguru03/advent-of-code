using advent_of_code.Common;

namespace advent_of_code._2023.day6;

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

    private static long Solve1(IEnumerable<Data> data)
    {
        return data.Aggregate(1, (acc, row) =>
        {
            var records = 0;
            for (var t = 1; t < row.Time; t++)
            {
                var distance = t * (row.Time - t);
                records += Convert.ToInt32(distance > row.Distance);
            }

            return acc * records;
        });
    }

    private static long Solve2(IEnumerable<Data> input)
    {
        var list = input.ToList();
        var data = new Data(Converter(list.Select(d => d.Time)), Converter(list.Select(d => d.Distance)));
        return Solve1(new List<Data> { data });

        long Converter(IEnumerable<long> nums) => long.Parse(string.Join("", nums.Select(num => num.ToString())));
    }

    private IEnumerable<Data> Parse(bool useExample)
    {
        return ParseText<IEnumerable<Data>>(useExample ? "example.txt" : "input.txt", text =>
        {
            var rows = text.Trim().Split("\n");
            var times = Parser(rows[0]);
            var distances = Parser(rows[1]);
            return times.Select((time, i) => new Data(time, distances[i]));

            List<long> Parser(string row) => row.Split(":")[1].Trim().Split(" ")
                .Where(str => !string.IsNullOrEmpty(str)).Select(long.Parse).ToList();
        });
    }

    private class Data
    {
        public long Time { get; }
        public long Distance { get; }

        public Data(long time, long distance)
        {
            Time = time;
            Distance = distance;
        }
    }
}
