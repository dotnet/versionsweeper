using DotNet.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SystemFile = System.IO.File;

namespace DotNet.Releases
{
    public class ProjectFileReader : IProjectFileReader
    {
        static readonly RegexOptions _options =
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture;
        static readonly Regex _targetFrameworkExpression =
            new(@"TargetFramework(.*)>(?<tfm>.+?)</", _options);

        public async ValueTask<Project> ReadProjectAsync(string projectPath)
        {
            if (SystemFile.Exists(projectPath))
            {
                var projectXml = await SystemFile.ReadAllTextAsync(projectPath);
                var match = _targetFrameworkExpression.Match(projectXml);
                var group = match.Groups["tfm"];
                var lineNumber = GetLineNumberFromIndex(projectXml, group.Index);
                var rawTfms = group.Value;

                return new()
                {
                    FullPath = projectPath,
                    TfmLineNumber = lineNumber,
                    RawTargetFrameworkMonikers = rawTfms,
                };
            }

            return new()
            {
                FullPath = projectPath
            };
        }

        static int GetLineNumberFromIndex(string xml, int index)
        {
            var lineNumber = 1;
            for (var i = 0; i < index; ++ i)
            {
                if (xml[i] == '\n') ++ lineNumber;
            }
            return lineNumber;
        }
    }
}
