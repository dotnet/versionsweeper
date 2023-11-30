// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.IO;

public sealed partial class SolutionFileReader(IProjectFileReader projectFileReader)
    : ISolutionFileReader
{
    public async ValueTask<Solution> ReadSolutionAsync(string solutionPath)
    {
        Solution solution = new();

        if (SystemFile.Exists(solutionPath))
        {
            solution.FullPath = Path.GetFullPath(solutionPath);
            string? solutionDirectory = Path.GetDirectoryName(solution.FullPath);
            string solutionText = await SystemFile.ReadAllTextAsync(solution.FullPath);
            MatchCollection matches = ProjectRegex().Matches(solutionText);

            foreach (Match match in matches.Cast<Match>())
            {
                string path = match.Groups["Path"].Value;
                string fullPath = Path.Combine(solutionDirectory!, path);

                if (SystemFile.Exists(fullPath) is false ||
                    SystemFile.GetAttributes(fullPath).HasFlag(FileAttributes.Directory))
                {
                    continue;
                }

                var project = await projectFileReader.ReadProjectAsync(fullPath);
                solution.Projects.Add(project);
            }
        }

        return solution;
    }

    [GeneratedRegex(
        pattern: "^Project\\(\"{(?<TypeId>[A-F0-9-]+)}\"\\) = \"(?<Name>.*?)\", \"(?<Path>.*?)\", \"{(?<Id>[A-F0-9-]+)}\"(?<Sections>(.|\\n|\\r)*?)EndProject(\\n|\\r)",
        options: RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture,
        cultureName: "en-US")]
    private static partial Regex ProjectRegex();
}
