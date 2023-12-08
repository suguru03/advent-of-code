using advent_of_code.Common;

namespace advent_of_code._2023.day7;

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
        var list = data.ToList();
        list.Sort((d1, d2) =>
        {
            if (d1.HandType != d2.HandType)
            {
                return d1.HandType.CompareTo(d2.HandType);
            }

            for (var i = 0; i < d1.Hands.Length; i++)
            {
                var left = d1.Hands[i];
                var right = d2.Hands[i];
                if (left != right)
                {
                    return left.CompareTo(right);
                }
            }

            return 0;
        });
        foreach (var item in list)
        {
            Console.WriteLine($"hand: {item.HandType}, cards: {string.Join(",", item.Hands)}");
        }

        return list.Select((t, i) => t.Bit * (i + 1)).Sum();
    }

    private IEnumerable<Data> Parse(bool useExample)
    {
        return ParseLines<Data>(useExample ? "example.txt" : "input.txt", text =>
        {
            var rows = text.Trim().Split(" ");
            var hands = rows[0]
                .Select(c => c switch
                {
                    'A' => 14,
                    'K' => 13,
                    'Q' => 12,
                    'J' => 11,
                    'T' => 10,
                    _ => int.Parse(c.ToString())
                }).ToList();
            var cardsMap = hands.Aggregate(new Dictionary<int, int>(), (dict, hand) =>
            {
                if (dict.ContainsKey(hand))
                {
                    dict[hand]++;
                }
                else
                {
                    dict[hand] = 1;
                }

                return dict;
            }).GroupBy(kvp => kvp.Value, kvp => kvp.Key).ToDictionary(g => g.Key);

            var handType = cardsMap.ContainsKey(5) ? HandType.FiveOfAKind :
                cardsMap.ContainsKey(4) ? HandType.FourOfAKind :
                cardsMap.ContainsKey(3) ? cardsMap.ContainsKey(2) ? HandType.FullHouse : HandType.ThreeOfAKind :
                cardsMap.TryGetValue(2, out var list) ? list.Count() == 2 ? HandType.TwoPair : HandType.OnePair :
                HandType.HighCard;

            var bit = int.Parse(rows[1]);
            return new Data(hands.ToArray(), handType, bit);
        });
    }

    private enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind,
    }

    private class Data
    {
        public int[] Hands { get; }
        public HandType HandType { get; }
        public int Bit { get; }

        public Data(int[] hands, HandType handType, int bit)
        {
            Hands = hands;
            HandType = handType;
            Bit = bit;
        }
    }
}
