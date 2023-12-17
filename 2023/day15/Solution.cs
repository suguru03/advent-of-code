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
        return data.List.Sum(str => str.Aggregate(0, Hash));
    }

    private static int Hash(int sum, char c)
    {
        return (sum + c) * Multiplier % Divisor;
    }

    private static int Solve2(Data data)
    {
        var boxMap = new Dictionary<int, Box>();
        foreach (var row in data.List)
        {
            var i = -1;
            var boxId = 0;
            while (++i < row.Length && row[i] is not ('=' or '-'))
            {
                boxId = Hash(boxId, row[i]);
            }

            if (!boxMap.TryGetValue(boxId, out var box))
            {
                box = new Box(boxId);
                boxMap.Add(boxId, box);
            }

            var key = row[..i];
            if (row[i] == '-')
            {
                box.Remove(key);
                continue;
            }

            box.Add(key, int.Parse(row[(i + 1)..]));
        }

        return boxMap.Values.Sum(box => box.GetTotalPower());
    }

    private Data Parse(bool useExample)
    {
        return new Data(ParseText(useExample ? "example.txt" : "input.txt", text => text.Trim().Split(',')));
    }

    private record Data(IEnumerable<string> List);

    private record Box(int BoxId)
    {
        private readonly List<(string key, int power)> items = new();

        public void Remove(string key)
        {
            var index = FindIndex(key);
            if (index < 0)
            {
                return;
            }

            items.RemoveAt(index);
        }

        public void Add(string key, int power)
        {
            var item = (key, power);
            var index = FindIndex(key);
            if (index < 0)
            {
                items.Add(item);
            }
            else
            {
                items[index] = item;
            }
        }

        public int GetTotalPower()
        {
            return items.Select((item, slot) => (BoxId + 1) * (slot + 1) * item.power).Sum();
        }

        private int FindIndex(string key)
        {
            return items.FindIndex(item => item.key == key);
        }
    }
}
