// inspired by https://github.com/tmbarker/advent-of-code

using System.CommandLine;
using advent_of_code.Common;

var rootCommand = new RootCommand();
var yearOption = new Option<int>(name: "--year");
var dayOption = new Option<int>(name: "--day");
var partOption = new Option<int>(name: "--part");
var exampleOption = new Option<bool>(name: "--example");
var testOption = new Option<bool>(name: "--test");
rootCommand.AddOption(yearOption);
rootCommand.AddOption(dayOption);
rootCommand.AddOption(partOption);
rootCommand.AddOption(exampleOption);
rootCommand.AddOption(testOption);
rootCommand.SetHandler(Run, symbol1: yearOption, symbol2: dayOption, symbol3: partOption, symbol4: exampleOption,
    symbol5: testOption);

await rootCommand.InvokeAsync(args);
return;

void Run(int year, int day, int part, bool useExample, bool runTest)
{
    Console.WriteLine($"Running {year}/{day} part: {part}, example: {useExample}");
    var target = string.Format($"advent_of_code._{year}.day{day}.Solution");
    var instance = (SolutionBase)Activator.CreateInstance(Type.GetType(target)!)!;
    instance.RootPath =
        Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../..",
            string.Format($"./{year}/day{day}")));
    if (runTest)
    {
        instance.Test();
        return;
    }

    var output = instance.Run(part, useExample);
    Console.WriteLine($"{output}");
}
