// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotNet.Extensions;
using DotNet.Models;
using Markdown;

namespace DotNet.GitHub
{
    public static class ModelExtensions
    {
        static (string FileName, HashSet<TargetFrameworkMonikerSupport> Tfms) AsNameSetPair(
            this ProjectSupportReport psr) =>
            (Path.GetFileName(psr.Project.FullPath), psr.TargetFrameworkMonikerSupports);

        static (string FileName, int ProjectsTargetingUnsupportedTfms) AsNameSetPair(
            this SolutionSupportReport psr) =>
            (Path.GetFileName(psr.Solution.FullPath), psr.ProjectSupportReports.Count(r => r.TargetFrameworkMonikerSupports.Any(t => t.IsUnsupported)));

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

        public static string ToTitleMessage(
            this SolutionSupportReport solutionSupportReport)
        {
            var (fileName, cnt) = solutionSupportReport.AsNameSetPair();
            return $"Update {cnt} projects in {fileName} to LTS (or current) version";
        }

        public static string ToMarkdownBody(
            this SolutionSupportReport solutionSupportReport,
            string rootDirectory,
            string branch)
        {
            IMarkdownDocument document = new MarkdownDocument();

            var (fileName, cnt) = solutionSupportReport.AsNameSetPair();

            document.AppendParagraph(
                $"There are {cnt} projects in *{fileName}* that target a .NET version which is no longer supported. " +
                "This is an auto-generated issue, detailed and discussed in [dotnet/docs#22271](https://github.com/dotnet/docs/issues/22271).");

            HashSet<string> relativePaths = new(StringComparer.OrdinalIgnoreCase);
            foreach (var psr in solutionSupportReport.ProjectSupportReports)
            {
                var project = psr.Project;
                var relativePath =
                    Path.GetRelativePath(rootDirectory, project.FullPath)
                        .Replace("\\", "/");
                relativePaths.Add(relativePath);

                var lineNumberFileReference =
                    $"../blob/{branch}/{relativePath}#L{project.TfmLineNumber}".EscapeUriString();

                var tfms = psr.TargetFrameworkMonikerSupports;
                document.AppendTable(
                    new MarkdownTableHeader(
                        new MarkdownTableHeaderCell("TFM in project"),
                        new MarkdownTableHeaderCell("Target version"),
                        new MarkdownTableHeaderCell("End of life"),
                        new MarkdownTableHeaderCell("Release notes"),
                        new MarkdownTableHeaderCell("Nearest LTS TFM version")),
                    tfms.Where(_ => _.IsUnsupported)
                        .Select(tfm =>
                        {
                            var (target, version, _, release) = tfm;
                            return new MarkdownTableRow(
                                $"[{Path.GetFileName(project.FullPath)}]({lineNumberFileReference})",
                                $"`{target}`",
                                release.EndOfLifeDate.HasValue ? $"{release.EndOfLifeDate:MMMM, dd yyyy}" : "N/A",
                                new MarkdownLink(
                                    $"{target} release notes", release.ReleaseNotesUrl)
                                    .ToString(),
                                $"`{tfm.NearestLtsVersion}`");
                        }));
            }

            document.AppendParagraph(
                "Consider upgrading to either the current release, or the nearest LTS TFM version.");

            document.AppendParagraph(
                $"If any of these projects are intentionally targeting an unsupported version, " +
                $"you can optionally configure to ignore this automated issue. " +
                $"Create a file at the root of the repository, named *dotnet-versionsweeper.json* and " +
                $"add an `ignore` entry following the " +
                $"[globbing patterns detailed here](https://docs.microsoft.com/dotnet/api/microsoft.extensions.filesystemglobbing.matcher#remarks).");

            var total = relativePaths.Count;
            document.AppendCode("json", @$"{{
    ""ignore"": [
{string.Join(Environment.NewLine, relativePaths.Select((relativePath, index) => $"        \"**/{relativePath}\"{(index == total ? "," : "")}"))}
    ]
}}");

            return document.ToString();
        }

        public static string ToMarkdownBody(
            this ProjectSupportReport psr,
            string rootDirectory,
            string branch)
        {
            IMarkdownDocument document = new MarkdownDocument();

            var (fileName, tfms) = psr.AsNameSetPair();

            document.AppendParagraph(
                $"The *{fileName}* project file targets a .NET version which is no longer supported. " +
                "This is an auto-generated issue, detailed and discussed in [dotnet/docs#22271](https://github.com/dotnet/docs/issues/22271).");

            var relativePath =
                Path.GetRelativePath(rootDirectory, psr.Project.FullPath)
                    .Replace("\\", "/");

            var lineNumberFileReference =
                $"../blob/{branch}/{relativePath}#L{psr.Project.TfmLineNumber}".EscapeUriString();

            document.AppendParagraph(
                $"See line {psr.Project.TfmLineNumber} in [{relativePath}]({lineNumberFileReference}).");

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
                            $"`{target}`",
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

            return document.ToString();
        }

        public static bool TryCreateIssueContent(
            this ISet<Project> projects,
            string rootDirectory,
            string branch,
            out (string Title, string MarkdownBody) result)
        {
            if (projects is { Count: 0 })
            {
                result = (null!, null!);
                return false;
            }

            IMarkdownDocument document = new MarkdownDocument();

            var uniqueProjects =
                projects.DistinctBy(project => project.FullPath, StringComparer.OrdinalIgnoreCase)
                    .OrderBy(project => project.FullPath)
                    .ToList();

            document.AppendParagraph(
                $"There are {uniqueProjects.Count} project(s) using the non-SDK-style project format. " +
                "This is an auto-generated issue, detailed and discussed in [dotnet/docs#22271](https://github.com/dotnet/docs/issues/22271).");

            static MarkdownCheckListItem AsCheckListItem(
                Project project, string root, string branch)
            {
                var relativePath =
                    Path.GetRelativePath(root, project.FullPath);

                var path = $"../blob/{branch}/{relativePath.Replace("\\", "/")}".EscapeUriString();
                var name = relativePath.FirstAndLastSegmentOfPath("...");

                return new(false, $"[{name}]({path})");
            }

            document.AppendList(
                new MarkdownList(
                    uniqueProjects
                        .Select(proj => AsCheckListItem(proj, rootDirectory, branch))));

            document.AppendParagraph(
                "Consider upgrading the project(s) to the [SDK-style format](https://docs.microsoft.com/dotnet/standard/frameworks).");

            document.AppendParagraph(
                $"If this project is intentionally demonstrating the old project style, " +
                $"you can optionally configure to ignore this automated issue. " +
                $"Create a file at the root of the repository, named *dotnet-versionsweeper.json* and " +
                $"add an `ignore` entry following the " +
                $"[globbing patterns detailed here](https://docs.microsoft.com/dotnet/api/microsoft.extensions.filesystemglobbing.matcher#remarks).");

            document.AppendCode("json", @"{
    ""ignore"": [
        ""**/Path/To/Example.csproj""
    ]
}");

            result =
                (
                    Title: "Upgrade project(s) to the SDK-style project format",
                    MarkdownBody: document.ToString()
                );

            return true;
        }
    }
}
