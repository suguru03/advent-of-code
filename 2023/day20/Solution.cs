using System.Text.RegularExpressions;
using advent_of_code.Common;

namespace advent_of_code._2023.day20;

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

    private static int Solve1(Data data)
    {
        const int times = 1_000;
        const string start = "button";
        const string firstReceiver = "broadcaster";
        var configMap = data.ConfigurationMap;
        var queue = new Queue<(string sender, string receiver, bool pulse)>();
        var counts = new[] { 0, 0 };
        var count = 0;
        while (++count <= times)
        {
            queue.Enqueue((start, firstReceiver, false));
            while (queue.Count != 0)
            {
                var (sender, receiver, pulse) = queue.Dequeue();
                counts[Convert.ToInt32(pulse)]++;
                if (!configMap.TryGetValue(receiver, out var config) || !config.Module.TryReceive(sender, pulse))
                {
                    continue;
                }

                foreach (var next in config.Receivers)
                {
                    queue.Enqueue((receiver, next, config.Module.Pulse));
                }
            }
        }

        return counts.Aggregate((left, right) => left * right);
    }

    private Data Parse(bool useExample)
    {
        var regex = ConfigurationRegex();
        var data = new Data(ParseLines(useExample ? "example.txt" : "input.txt", text =>
        {
            var groups = regex.Match(text).Groups;
            var id = groups["Id"].Value;
            return new Configuration(groups["Type"].Value switch
            {
                "%" => new FlipFlopModule(id),
                "&" => new ConjunctionModule(id),
                _ => new Module(id),
            }, groups["Destinations"].Value.Split(',').Select(dest => dest.Trim()));
        }).ToDictionary(config => config.Module.Id));

        foreach (var (sender, (_, receivers)) in data.ConfigurationMap)
        foreach (var receiver in receivers)
        {
            if (!data.ConfigurationMap.TryGetValue(receiver, out var config) ||
                config.Module is not ConjunctionModule conjunctionModule)
            {
                continue;
            }

            conjunctionModule.RegisterConnectedId(sender);
        }

        return data;
    }

    private record Data(Dictionary<string, Configuration> ConfigurationMap);

    private record Configuration(Module Module, IEnumerable<string> Receivers);

    private class Module(string id)
    {
        public string Id => id;
        public virtual bool Pulse { get; protected set; }

        public virtual bool TryReceive(string sender, bool pulse)
        {
            return true;
        }
    };

    private class FlipFlopModule(string id) : Module(id)
    {
        public override bool TryReceive(string sender, bool pulse)
        {
            if (pulse)
            {
                return false;
            }

            Pulse ^= true;
            return true;
        }
    }

    private class ConjunctionModule(string id) : Module(id)
    {
        private readonly HashSet<string> _connectedIdSet = [];
        private readonly HashSet<string> _highPulseIdSet = [];
        public override bool Pulse => _highPulseIdSet.Count != _connectedIdSet.Count;

        public void RegisterConnectedId(string id)
        {
            _connectedIdSet.Add(id);
        }

        public override bool TryReceive(string sender, bool pulse)
        {
            if (pulse)
            {
                _highPulseIdSet.Add(sender);
            }
            else
            {
                _highPulseIdSet.Remove(sender);
            }

            return true;
        }
    }

    [GeneratedRegex(@"(?<Type>-?[%&]?)(?<Id>-?.+) -> (?<Destinations>-?.+)")]
    private static partial Regex ConfigurationRegex();
}
