using advent_of_code.Common;

namespace advent_of_code._2023.day15;

public class Solution : SolutionBase
{
    private const int Multiplier = 17;
    private const int Divisor = 256;

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
        return data.List.Select(str => str.Aggregate(0, (sum, c) => (sum + c) * Multiplier % Divisor)).Sum();
    }

    private static int Solve2(Data data)
    {
        var boxes = new int[256].Select(_ => new List<(string key, int power)>()).ToList();
        foreach (var row in data.List)
        {
            var i = -1;
            var boxId = 0;
            while (++i < row.Length && row[i] is not ('=' or '-'))
            {
                boxId = (boxId + row[i]) * Multiplier % Divisor;
            }

            var box = boxes[boxId];
            var key = row[..i];
            var index = box.FindIndex(item => item.key == key);
            if (row[i] == '-')
            {
                if (index >= 0)
                {
                    box.RemoveAt(index);
                }

                continue;
            }

            var item = (key, int.Parse(row[(i + 1)..]));
            if (index < 0)
            {
                box.Add(item);
            }
            else
            {
                box[index] = item;
            }
        }

        return boxes.Select((l, boxId) => l.Select((i, slot) => (boxId + 1) * (slot + 1) * i.power).Sum()).Sum();
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseText(useExample ? "example.txt" : "input.txt", text => text.Trim().Split(',')));
    }

    private readonly record struct Data(IEnumerable<string> List);
}
