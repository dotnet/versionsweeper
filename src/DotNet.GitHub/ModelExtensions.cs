using DotNet.Models;
using Markdown;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNet.GitHub
{
    public static class ModelExtensions
    {
        static (string FileName, HashSet<TargetFrameworkMonikerSupport> Tfms) AsNameSetPair(
            this ProjectSupportReport psr) =>
            (Path.GetFileName(psr.ProjectPath), psr.TargetFrameworkMonikerSupports);

        public static string ToTitleMessage(
            this ProjectSupportReport projectSupportReport)
        {
            StringBuilder builder = new();

            var (fileName, tfms) = projectSupportReport.AsNameSetPair();

            builder.Append($"Update {fileName} from ");
            var message =
                string.Join(
                    ", ",
                    tfms.Where(tfm => tfm.IsUnsupported)
                        .Select(tfm => tfm.Release.ToBrandString()));
            builder.Append(message);
            builder.Append(" to LTS (or current) version");

            return builder.ToString();
        }

        public static string ToMarkdownBody(
            this ProjectSupportReport projectSupportReport,
            string rootDirectory,
            int tfmLineNumber)
        {
            IMarkdownDocument document = new MarkdownDocument();

            var (fileName, tfms) = projectSupportReport.AsNameSetPair();

            document.AppendParagraph(
                $"The *{fileName}* project file targets a .NET version which is no longer supported. " +
                "This is an auto-generated issue, detailed and discussed in [dotnet/docs#22271](https://github.com/dotnet/docs/issues/22271).");

            var relativePath =
                Path.GetRelativePath(rootDirectory, projectSupportReport.ProjectPath)
                    .Replace("\\", "/");

            var lineNumberFileReference =
                $"../blob/master/{relativePath}#L{tfmLineNumber}";

            document.AppendParagraph(
                $"See line {tfmLineNumber} in [{relativePath}]({lineNumberFileReference}).");



            document.AppendTable(
                new MarkdownTableHeader(
                    new MarkdownTableHeaderCell("Target version"),
                    new MarkdownTableHeaderCell("End of life"),
                    new MarkdownTableHeaderCell("Release notes"),
                    new MarkdownTableHeaderCell("Nearest LTS TFM version")),
                tfms.Where(_ => _.IsUnsupported)
                    .Select(tfm =>
                    {
                        var (target, version, _, release) = tfm;
                        return new MarkdownTableRow(
                            target,
                            release.EndOfLifeDate.HasValue ? $"{release.EndOfLifeDate:MMMM, dd yyyy}" : "N/A",
                            new MarkdownLink(
                                $"{target} release notes", release.ReleaseNotesUrl)
                                .ToString(),
                            $"`{tfm.NearestLtsVersion}`");
                    }));

            document.AppendParagraph(
                "Consider upgrading the project to either the current release, or the nearest LTS TFM version.");

            document.AppendParagraph(
                $"If this project is intentionally targeting an unsupported version, " +
                $"you can optionally configure to ignore this automated issue. " +
                $"Create a file at the root of the repository, named *dotnet-versionsweeper.json* and " +
                $"add an `ignore` entry following the " +
                $"[globbing patterns detailed here](https://docs.microsoft.com/dotnet/api/microsoft.extensions.filesystemglobbing.matcher#remarks).");

            document.AppendCode("json", @$"{{
    ""ignore"": [
        ""**/{relativePath}""
    ]
}}");

            // TODO: add a link to example configs, once available.

            return document.ToString();
        }
    }
}
