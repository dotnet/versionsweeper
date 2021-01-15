using DotNet.Versions.Records;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SystemFile = System.IO.File;

namespace DotNet.Versions
{
    internal class ProjectFileReader : IProjectFileReader
    {
        static readonly string[] _emptyArray = Array.Empty<string>();
        static readonly RegexOptions _options =
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture;
        static readonly Regex _targetFrameworkExpression =
            new(@"TargetFramework(.*)>(?<tfm>.+?)</", _options);

        async Task<string[]> IProjectFileReader.ReadProjectTfmsAsync(string filePath)
        {
            if (SystemFile.Exists(filePath))
            {
                var projectXml = await SystemFile.ReadAllTextAsync(filePath);
                var match = _targetFrameworkExpression.Match(projectXml);
                var value = match.Groups["tfm"].Value;

                return value?.Split(";", StringSplitOptions.RemoveEmptyEntries) ?? _emptyArray;
            }

            return _emptyArray;
        }
    }
}
