using DotNet.Extensions;
using DotNet.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SystemFile = System.IO.File;

namespace DotNet.IO
{
    public class ProjectFileReader : IProjectFileReader
    {
        static readonly RegexOptions _options =
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture;
        static readonly Regex _projectSdkExpression =
            new(@"\<Project Sdk=""(?<sdk>.+?)""", _options);
        static readonly Regex _targetFrameworkExpression =
            new(@"TargetFramework(.*)>(?<tfm>.+?)</", _options);

        public async ValueTask<Project> ReadProjectAsync(string projectPath)
        {
            Project project = new()
            {
                FullPath = projectPath
            };

            if (SystemFile.Exists(projectPath))
            {
                var projectXml = await SystemFile.ReadAllTextAsync(projectPath);
                var (index, rawTfms) = MatchExpression(_targetFrameworkExpression, projectXml, "tfm");
                var lineNumber = GetLineNumberFromIndex(projectXml, index);
                var (_, sdk) = MatchExpression(_projectSdkExpression, projectXml, "sdk");

                return project with
                {
                    TfmLineNumber = lineNumber,
                    RawTargetFrameworkMonikers = rawTfms!,
                    Sdk = sdk.NullIfEmpty()
                };
            }

            return project;
        }

        static (int Index, string? Value) MatchExpression(
            Regex expression, string content, string groupName)
        {
            var match = expression?.Match(content);
            if (match is not null)
            {
                var group = match.Groups[groupName];
                return (group.Index, group.Value);
            }

            return (0, null);
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
