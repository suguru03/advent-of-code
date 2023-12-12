using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2023.day8;

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

    private static long Solve1(Data data)
    {
        return Count(data, "AAA", currentId => currentId == "ZZZ");
    }

    private static long Solve2(Data data)
    {
        return data.NodeMap.Keys.Where(key => key.EndsWith('A'))
            .Select(id => Count(data, id, currentId => currentId.EndsWith('Z'))).Aggregate(1L, Utils.Math.Lcm);
    }

    private static long Count(Data data, string currentId, Func<string, bool> isEnd)
    {
        var instruction = data.Instruction;
        var nodeMap = data.NodeMap;
        var index = -1;
        var count = 0;
        while (!isEnd(currentId))
        {
            count++;
            index = (index + 1) % instruction.Length;
            var current = nodeMap[currentId];
            currentId = instruction[index] == 'L' ? current.Left : current.Right;
        }

        return count;
    }

    private Data Parse(int part, bool useExample)
    {
        return ParseText<Data>(useExample ? $"example{part}.txt" : "input.txt", text =>
        {
            var rows = text.Trim().Split("\n");
            var instruction = rows[0].Trim();
            var nodeMap = rows.Skip(2).Where(str => !string.IsNullOrEmpty(str)).Select(str =>
            {
                var groups = Regex.Match(str, @"(.+) = \((.+), (.+)\)").Groups;
                return new Node(groups[1].Value.Trim(), groups[2].Value.Trim(), groups[3].Value.Trim());
            }).ToDictionary(n => n.Id);
            return new Data(instruction, nodeMap);
        });
    }

    private class Node
    {
        public string Id { get; }
        public string Left { get; }
        public string Right { get; }

        public Node(string id, string left, string right)
        {
            Id = id;
            Left = left;
            Right = right;
        }
    }

    private class Data
    {
        public string Instruction { get; }
        public Dictionary<string, Node> NodeMap { get; }

        public Data(string instruction, Dictionary<string, Node> nodeMap)
        {
            Instruction = instruction;
            NodeMap = nodeMap;
        }
    }
}
