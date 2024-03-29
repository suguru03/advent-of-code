namespace advent_of_code.Common;

public abstract class SolutionBase
{
    protected const string ProblemNotSolvedString = "Problem not solved!";
    public string RootPath { private get; set; } = "";
    public abstract object Run(int part, bool useExample);

    public virtual void Test()
    {
        throw new NotImplementedException();
    }

    private string ResolvePath(string filepath)
    {
        return $"{RootPath}/{filepath}";
    }

    protected T ParseText<T>(string filepath, Func<string, T> parser)
    {
        return parser(File.ReadAllText(ResolvePath(filepath)));
    }

    protected IEnumerable<string> ParseLines(string filepath)
    {
        return File.ReadLines(ResolvePath(filepath));
    }

    protected IEnumerable<T> ParseLines<T>(string filepath, Func<string, T> parser)
    {
        return File.ReadLines(ResolvePath(filepath)).Select(parser);
    }
}
