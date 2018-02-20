using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NugetAnalyzer.Core.Models;
using static System.Environment;
using static System.IO.Directory;
using static System.IO.Path;

namespace NugetAnalyzer.Core
{
    public sealed class SolutionAnalyzer
    {
        public OperationResult<Package[]> Analyze()
        {
            var solutionFiles = EnumerateFiles(CurrentDirectory, "*.sln").ToArray();

            if (solutionFiles.Skip(1).Any())
                return OperationResult.Error<Package[]>(new Exception("More than one solution file was found."));

            var solutionFile = solutionFiles.FirstOrDefault();

            var projectFolders = GetProjectFolders(File.ReadAllText(solutionFile));
            var packagesConfigs = projectFolders.SelectMany(pf => EnumerateFiles(pf, "packages.config")).ToArray();

            var packages = packagesConfigs.Select(pc => XDocument.Parse(File.ReadAllText(pc)))
                .SelectMany(xml => xml.Descendants("package"))
                .AsParallel()
                .Select(packageNode => new Package(packageNode.Attribute("id").Value, packageNode.Attribute("version").Value, packageNode.Attribute("targetFramework").Value))
                .Distinct()
                .OrderBy(p => p.Id)
                .ToArray();

            return OperationResult.Success(packages);
        }

        private IEnumerable<string> GetProjectFolders(string solutionFile)
        {
            var projectMatches = Regex.Matches(solutionFile, @"Project.+=\s"".+?"",\s""(?<projectPath>.+?)""");
            foreach (Match projectMatch in projectMatches)
                yield return GetDirectoryName(Combine(CurrentDirectory, projectMatch.Groups["projectPath"].Value));
        }
    }
}
