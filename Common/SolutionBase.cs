namespace advent_of_code.Common;

public abstract class SolutionBase
{
    protected const string ProblemNotSolvedString = "Problem not solved!";
    public string RootPath { private get; set; } = "";
    public abstract object Run(int part, bool useExample);

    private string ResolvePath(string filepath)
    {
        return $"{RootPath}/{filepath}";
    }

    protected IEnumerable<T> ParseLines<T>(string filepath, Func<string, T> parser)
    {
        return File.ReadLines(ResolvePath(filepath)).Select(parser);
    }
}
