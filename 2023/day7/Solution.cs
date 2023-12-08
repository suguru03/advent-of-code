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
            2 => Solve2(input),
            _ => ProblemNotSolvedString
        };
    }

    private static long Solve1(IEnumerable<Data> data)
    {
        var list = data.Select(row =>
        {
            var cardCountMap = row.Hands.Aggregate(new Dictionary<int, int>(), (dict, hand) =>
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
            });
            return (data: row, handType: GetHandType(cardCountMap));
        });
        return Sum(list, hand => (hand + 13 - 2) % 13);
    }

    private static HandType GetHandType(IDictionary<int, int> cardCountMap)
    {
        var cardsMap = cardCountMap.GroupBy(kvp => kvp.Value, kvp => kvp.Key).ToDictionary(g => g.Key);
        return cardsMap.ContainsKey(5) ? HandType.FiveOfAKind :
            cardsMap.ContainsKey(4) ? HandType.FourOfAKind :
            cardsMap.ContainsKey(3) ? cardsMap.ContainsKey(2) ? HandType.FullHouse : HandType.ThreeOfAKind :
            cardsMap.TryGetValue(2, out var list) ? list.Count() == 2 ? HandType.TwoPair : HandType.OnePair :
            HandType.HighCard;
    }

    private static long Solve2(IEnumerable<Data> data)
    {
        var list = data.Select(row =>
        {
            var max = 0;
            var maxHand = 0;
            var cardCountMap = row.Hands.Aggregate(new Dictionary<int, int>(), (dict, hand) =>
            {
                if (dict.ContainsKey(hand))
                {
                    dict[hand]++;
                }
                else
                {
                    dict[hand] = 1;
                }

                if (dict[hand] <= max || hand == 11)
                {
                    return dict;
                }

                max = dict[hand];
                maxHand = hand;
                return dict;
            });

            cardCountMap.TryGetValue(11, out var jack);
            if (jack is 0 or 5)
            {
                return (data: row, handType: GetHandType(cardCountMap));
            }

            cardCountMap.Remove(11);
            cardCountMap[maxHand] += jack;
            return (data: row, handType: GetHandType(cardCountMap));
        });

        return Sum(list, hand => hand switch
        {
            1 => 14,
            11 => 0,
            _ => hand
        });
    }

    private static int Sum(IEnumerable<(Data data, HandType handType)> enumerable, Func<int, int> handConverter)
    {
        var list = enumerable.ToList();
        list.Sort((d1, d2) =>
        {
            if (d1.handType != d2.handType)
            {
                return d1.handType.CompareTo(d2.handType);
            }

            for (var i = 0; i < d1.data.Hands.Length; i++)
            {
                var left = handConverter(d1.data.Hands[i]);
                var right = handConverter(d2.data.Hands[i]);
                if (left != right)
                {
                    return left.CompareTo(right);
                }
            }

            return 0;
        });

        return list.Select((t, i) => t.data.Bit * (i + 1)).Sum();
    }

    private IEnumerable<Data> Parse(bool useExample)
    {
        return ParseLines<Data>(useExample ? "example.txt" : "input.txt", text =>
        {
            var rows = text.Trim().Split(" ");
            var hands = rows[0]
                .Select(c => c switch
                {
                    'A' => 1,
                    'K' => 13,
                    'Q' => 12,
                    'J' => 11,
                    'T' => 10,
                    _ => int.Parse(c.ToString())
                }).ToList();
            var bit = int.Parse(rows[1]);
            return new Data(hands.ToArray(), bit);
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
        public int Bit { get; }

        public Data(int[] hands, int bit)
        {
            Hands = hands;
            Bit = bit;
        }
    }
}
