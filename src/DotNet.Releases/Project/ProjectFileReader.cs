using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SystemFile = System.IO.File;

namespace DotNet.Releases
{
    public class ProjectFileReader : IProjectFileReader
    {
        static readonly string[] _emptyArray = Array.Empty<string>();
        static readonly RegexOptions _options =
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture;
        static readonly Regex _targetFrameworkExpression =
            new(@"TargetFramework(.*)>(?<tfm>.+?)</", _options);

        public async ValueTask<(int LineNumber, string[] Tfms)> ReadProjectTfmsAsync(string filePath)
        {
            if (SystemFile.Exists(filePath))
            {
                var projectXml = await SystemFile.ReadAllTextAsync(filePath);
                var match = _targetFrameworkExpression.Match(projectXml);
                var group = match.Groups["tfm"];
                var lineNumber = GetLineNumberFromIndex(projectXml, group.Index);
                var value = group.Value;

                return (lineNumber, value?.Split(";", StringSplitOptions.RemoveEmptyEntries) ?? _emptyArray);
            }

            return (-1, _emptyArray);
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
