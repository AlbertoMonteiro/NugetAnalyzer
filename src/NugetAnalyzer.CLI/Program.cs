using System;
using NugetAnalyzer.Core;

namespace NugetAnalyzer.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var solutionAnalyzer = new SolutionAnalyzer();

            var operationResult = solutionAnalyzer.Analyze();

            foreach (var package in operationResult.Result)
            {
                Console.Write($"Your app depends on {package.Id} at {package.Version}");
                if (package.IsOutdated)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($" and its outdated, the latest version is: {package.LastestVersion}");
                    Console.ResetColor();
                }
                else if (!package.IsOutdated)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" and its updated");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
    }
}
