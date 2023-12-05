// inspired by https://github.com/tmbarker/advent-of-code

using System.CommandLine;
using advent_of_code.Common;

var rootCommand = new RootCommand();
var yearOption = new Option<int>(name: "--year");
var dayOption = new Option<int>(name: "--day");
var partOption = new Option<int>(name: "--part");
var exampleOption = new Option<bool>(name: "--example");
rootCommand.AddOption(yearOption);
rootCommand.AddOption(dayOption);
rootCommand.AddOption(partOption);
rootCommand.AddOption(exampleOption);
rootCommand.SetHandler(Run, symbol1: yearOption, symbol2: dayOption, symbol3: partOption, symbol4: exampleOption);

await rootCommand.InvokeAsync(args);

void Run(int year, int day, int part, bool useExample)
{
    Console.WriteLine($"Running {year}/{day} part: {part}, example: {useExample}");
    var target = string.Format($"advent_of_code._{year}.day{day}.Solution");
    var instance = (SolutionBase)Activator.CreateInstance(Type.GetType(target)!)!;
    instance.RootPath =
        Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../..",
            string.Format($"./{year}/day{day}")));
    var output = instance.Run(part, useExample);
    Console.WriteLine($"{output}");

}
