using System.Text.RegularExpressions;
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
            _ => ProblemNotSolvedString
        };
    }

    private static long Solve1(IEnumerable<Data> data)
    {
        return data.Aggregate(1, (acc, data) =>
        {
            var records = 0;
            for (var t = 1; t < data.Time; t++)
            {
                var distance = t * (data.Time - t);
                if (distance > data.Distance)
                {
                    records++;
                }
            }

            return acc * records;
        });
    }

    private IEnumerable<Data> Parse(bool useExample)
    {
        return ParseText<IEnumerable<Data>>(useExample ? "example.txt" : "input.txt", text =>
        {
            var rows = text.Trim().Split("\n");

            var times = Parser(rows[0]);
            var distances = Parser(rows[1]);
            var arr = new Data[times.Count];
            for (var i = 0; i < times.Count; i++)
            {
                arr[i] = new Data(times[i], distances[i]);
            }

            return arr;

            List<int> Parser(string text) => text.Split(":")[1].Trim().Split(" ")
                .Where(str => !string.IsNullOrEmpty(str)).Select(int.Parse).ToList();
        });
    }

    private class Data
    {
        public int Time { get; }
        public int Distance { get; }

        public Data(int time, int distance)
        {
            Time = time;
            Distance = distance;
        }
    }
}
