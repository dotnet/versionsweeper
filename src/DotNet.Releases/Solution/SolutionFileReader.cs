using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SystemFile = System.IO.File;

namespace DotNet.Releases.Solution
{
    public class SolutionFileReader : ISolutionFileReader
    {
        static readonly RegexOptions _options =
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture;
        static readonly Regex _solutionProjectsExpression =
            new("^Project\\(\"{(?<TypeId>[A-F0-9-]+)}\"\\) = " +
                "\"(?<Name>.*?)\", " +
                "\"(?<Path>.*?)\", " +
                "\"{(?<Id>[A-F0-9-]+)}\"" +
                @"(?<Sections>(.|\n|\r)*?)" +
                @"EndProject(\n|\r)",
                _options);

        public async ValueTask<Solution> ReadSolutionAsync(string solutionPath)
        {
            Solution solution = new();

            if (SystemFile.Exists(solutionPath))
            {
                solution.Path = Path.GetFullPath(solutionPath);
                var solutionDirectory = Path.GetDirectoryName(solution.Path);
                var solutionText = await SystemFile.ReadAllTextAsync(solution.Path);
                var matches = _solutionProjectsExpression.Matches(solutionText);
                
                foreach (Match match in matches)
                {
                    var name = match.Groups["Name"].Value;
                    var path = match.Groups["Path"].Value;
                    var fullPath = Path.Combine(solutionDirectory!, path);
                    if (SystemFile.Exists(fullPath) is false ||
                        SystemFile.GetAttributes(fullPath).HasFlag(FileAttributes.Directory))
                    {
                        continue;
                    }
                    
                    solution.Projects[name] = fullPath;
                }
            }

            return solution;
        }
    }
}
