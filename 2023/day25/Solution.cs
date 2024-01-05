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
        return MinCut(graph);
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
        }

        Console.WriteLine($"best: {string.Join(',', bestPartition)}, cut: S: {bestCut!.S}, T: {bestCut.T}, Weight: {bestCut.Weight}");
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
        var start = graph.GetVertexSet().First();
        var founds = new List<string>(collection: [start]);
        var candidateSet = graph.GetVertexSet().ToHashSet();
        var remaining = candidateSet.Count;
        candidateSet.Remove(start);
        var cutWeight = 0;
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
            cutWeight = max;
        }

        var n = founds.Count;
        return new CutOfThePhase(founds[n - 2], founds[n - 1], cutWeight);
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

        public IEnumerable<string> GetVertexSet()
        {
            return edgesMap.Keys;
        }

        public int GetVertexSize()
        {
            return GetVertexSet().Count();
        }
    }

    private record CutOfThePhase(string S, string T, int Weight);

    [GeneratedRegex(@"(?<Name>-?.+):(?<Connection>-?.+)")]
    private static partial Regex RowRegex();
}
