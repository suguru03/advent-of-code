using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2023.day25;

public partial class Solution : SolutionBase
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

    private static int Solve1(Graph graph)
    {
        return Solve(graph);
    }

    /// <summary>
    /// https://www.reddit.com/r/adventofcode/comments/18qbsxs/comment/ketzp94/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
    /// </summary>
    /// <param name="graph"></param>
    /// <returns></returns>
    private static int Solve(Graph graph)
    {
        var vertexSize = graph.GetVertexSize();
        var vertexSet = graph.GetVertexSet();
        while (vertexSet.Sum(Count) != 3)
        {
            Console.WriteLine($"{vertexSet.Sum(Count)}, {string.Join(',', vertexSet.OrderBy(Count))}");
            vertexSet.Remove(vertexSet.OrderBy(Count).Last());
        }

        return vertexSet.Count * (vertexSize - vertexSet.Count);

        int Count(string vertex)
        {
            return graph.GetEdgeMap(vertex).Keys.Count(v => !vertexSet.Contains(v));
        }
    }

    private static int MinCut(Graph orig)
    {
        var graph = orig.Copy();
        var partition = new HashSet<string>();
        var bestPartition = partition;
        CutOfThePhase? bestCut = null;
        while (graph.GetVertexSize() > 2)
        {
            var cutOfThePhase = MaximumAdjacencySearch(graph);
            partition.Add(cutOfThePhase.T);
            if (bestCut == null || cutOfThePhase.Weight < bestCut.Weight)
            {
                bestCut = cutOfThePhase;
                bestPartition = [..partition];
            }

            MergeVertexFromCut(graph, cutOfThePhase);

            if (bestCut.Weight == 3)
            {
                break;
            }
        }

        Console.WriteLine($"best: {string.Join(',', bestPartition)}, cut: S: {bestCut!.S}, T: {bestCut.T}, Weight: {bestCut.Weight}");
        // 532700 too low
        return bestPartition.Count * (orig.GetVertexSize() - bestPartition.Count);
    }

    private static void MergeVertexFromCut(Graph graph, CutOfThePhase cutOfThePhase)
    {
        var (s, t, _) = cutOfThePhase;
        var edgeMap = graph.GetEdgeMap(t);
        graph.DeleteVertex(t);
        foreach (var (destination, edge) in edgeMap)
        {
            if (s == destination)
            {
                continue;
            }

            graph.AddWeight(s, destination, edge.Weight);
        }
    }

    private static CutOfThePhase MaximumAdjacencySearch(Graph graph)
    {
        var start = graph.GetVertexSet().Last();
        var founds = new List<string>(collection: [start]);
        var candidateSet = graph.GetVertexSet().ToHashSet();
        var remaining = candidateSet.Count;
        candidateSet.Remove(start);
        var cutWeights = new List<int>();
        while (candidateSet.Count != 0)
        {
            var max = -1;
            var target = start;
            Console.WriteLine($"Remaining: {candidateSet.Count}/{remaining}");
            foreach (var candidate in candidateSet)
            {
                var sum = founds.Sum(source => graph.GetWeight(source, candidate));
                if (sum <= max)
                {
                    continue;
                }

                max = sum;
                target = candidate;
            }

            candidateSet.Remove(target);
            founds.Add(target);
            cutWeights.Add(max);
        }

        var n = founds.Count;
        return new CutOfThePhase(founds[n - 2], founds[n - 1], cutWeights.Last());
    }


    private Graph Parse(bool useExample)
    {
        var rowRegex = RowRegex();
        var graph = new Graph(new Dictionary<string, Dictionary<string, Edge>>());
        foreach (var text in ParseLines(useExample ? "example.txt" : "input.txt"))
        {
            var group = rowRegex.Match(text).Groups;
            var source = group["Name"].Value;
            var connections = group["Connection"].Value.Trim().Split(' ');
            foreach (var connection in connections)
            {
                graph.SetWeight(source, connection, 1);
            }
        }

        return graph;
    }

    private record Edge(string Source, string Destination, int Weight);

    private class Graph(IDictionary<string, Dictionary<string, Edge>> edgesMap)
    {
        public Graph Copy()
        {
            return new Graph(edgesMap.ToDictionary(kvp => kvp.Key, kvp => new Dictionary<string, Edge>(kvp.Value)));
        }

        public Dictionary<string, Edge> GetEdgeMap(string source)
        {
            return edgesMap[source];
        }

        private Edge? GetEdge(string source, string destination)
        {
            return GetEdgeMap(source).GetValueOrDefault(destination);
        }

        public int GetWeight(string source, string destination)
        {
            return GetEdge(source, destination)?.Weight ?? 0;
        }

        public void SetWeight(string source, string destination, int weight)
        {
            var edge = new Edge(source, destination, weight);
            SetEdge(source, destination, edge);
            SetEdge(destination, source, edge);
        }

        public void AddWeight(string source, string destination, int weight)
        {
            SetWeight(source, destination, GetWeight(source, destination) + weight);
        }

        private void SetEdge(string source, string destination, Edge edge)
        {
            if (!edgesMap.TryGetValue(source, out var map))
            {
                map = new Dictionary<string, Edge>();
                edgesMap[source] = map;
            }

            edgesMap[source][destination] = edge;
        }

        public void DeleteVertex(string source)
        {
            var edgeMap = edgesMap[source];
            edgesMap.Remove(source);
            foreach (var destination in edgeMap.Keys)
            {
                edgesMap[destination].Remove(source);
            }
        }

        public HashSet<string> GetVertexSet()
        {
            return edgesMap.Keys.ToHashSet();
        }

        public int GetVertexSize()
        {
            return edgesMap.Count;
        }
    }

    private record CutOfThePhase(string S, string T, int Weight);

    [GeneratedRegex(@"(?<Name>-?.+):(?<Connection>-?.+)")]
    private static partial Regex RowRegex();
}
