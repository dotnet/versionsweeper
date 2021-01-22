using DotNet.Models;
using Markdown;
using System.IO;
using System.Linq;

namespace DotNet.GitHub
{
    public static class ModelExtensions
    {
        public static string ToIssueBody(
            this ProjectSupportReport projectSupportReport)
        {
            IMarkdownDocument document = new MarkdownDocument();

            var (projectPath, tfms) = projectSupportReport;
            var fileName = Path.GetFileName(projectPath);

            document.AppendHeader(
                $"Project file *{fileName}* targets a .NET version which is no longer supported", 3);

            document.AppendParagraph(
                "This is an auto-generated issue, discussed in further detail [here](https://github.com/dotnet/docs/issues/22271).");

            document.AppendTable(
                new MarkdownTableHeader(
                    new MarkdownTableHeaderCell("Target Version"),
                    new MarkdownTableHeaderCell("End of life"),
                    new MarkdownTableHeaderCell("Release notes")),
                tfms.Where(_ => _.IsUnsupported)
                    .Select(tfm =>
                    {
                        var (target, version, _, release) = tfm;
                        return new MarkdownTableRow(
                            target,
                            $"{release.EndOfLifeDate:MMMM, dd yyyy}",
                            );
                    }));

            document.AppendParagraph(
                "Consider upgrading the project to either the current release, or the next closest LTS version.");

            return document.ToString();
        }
    }
}
