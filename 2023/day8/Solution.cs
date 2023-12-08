using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2023.day8;

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

    private static int Solve1(Data data)
    {
        var instruction = data.Instruction;
        var nodeMap = data.NodeMap;
        var index = -1;
        var count = 0;
        var currentId = "AAA";
        const string endId = "ZZZ";
        while (currentId != endId)
        {
            count++;
            index = (index + 1) % instruction.Length;
            var current = nodeMap[currentId];
            currentId = instruction[index] == 'L' ? current.Left : current.Right;
        }

        return count;
    }

    private Data Parse(bool useExample)
    {
        return ParseText<Data>(useExample ? "example.txt" : "input.txt", text =>
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
