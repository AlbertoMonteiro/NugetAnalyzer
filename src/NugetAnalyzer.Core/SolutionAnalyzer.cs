using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Environment;
using static System.IO.Directory;
using static System.IO.Path;

namespace NugetAnalyzer.Core
{
    public class SolutionAnalyzer
    {
        public OperationResult Analyze()
        {
            var solutionFiles = EnumerateFiles(CurrentDirectory, "*.sln");

            if (solutionFiles.Skip(1).Any())
                return new OperationResult("More than one solution file was found.");

            var solutionFile = solutionFiles.FirstOrDefault();

            var projectFolders = GetProjectFolders(solutionFile);
            var packagesConfigs = projectFolders.SelectMany(pf => EnumerateFiles(pf, "packages.config")).ToArray();
        }

        private IEnumerable<string> GetProjectFolders(string solutionFile)
        {
            var projectMatches = Regex.Matches(solutionFile, @"Project.+=\s"".+?"",\s""(?<projectPath>.+?)""");
            foreach (Match projectMatch in projectMatches)
                yield return GetDirectoryName(Combine(CurrentDirectory, projectMatch.Groups["projectPath"].Value));
        }
    }

    public struct OperationResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }

        public OperationResult(string error) : this()
        {
            Error = error;
        }
    }
}
