// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static string ToMarkdownBody(
            this ISet<ProjectSupportReport> psrs,
            string tfm,
            string rootDirectory,
            string branch)
        {
            IMarkdownDocument document = new MarkdownDocument();

            document.AppendParagraph(
                "The following project file(s) target a .NET version which is no longer supported. " +
                "This is an auto-generated issue, detailed and discussed in [dotnet/docs#22271](https://github.com/dotnet/docs/issues/22271).");

            var tfmSupport =
                psrs.First()
                    .TargetFrameworkMonikerSupports
                    .First(tfms => tfms.TargetFrameworkMoniker == tfm);

            document.AppendTable(
                new MarkdownTableHeader(
                    new("Target version"),
                    new("End of life"),
                    new("Release notes"),
                    new("Nearest LTS TFM version")),
                new[]
                {
                    new MarkdownTableRow(
                    $"`{tfmSupport.TargetFrameworkMoniker}`",
                    tfmSupport.Release.EndOfLifeDate.HasValue
                        ? $"{tfmSupport.Release.EndOfLifeDate:MMMM, dd yyyy}" : "N/A",
                    new MarkdownLink(
                        $"{tfmSupport.TargetFrameworkMoniker} release notes", tfmSupport.Release.ReleaseNotesUrl)
                        .ToString(),
                    $"`{tfmSupport.NearestLtsVersion}`")
                });

            document.AppendList(
                new MarkdownList(
                    psrs.OrderBy(psr => psr.Project.FullPath).Select(psr =>
                    {
                        var relativePath =
                            Path.GetRelativePath(rootDirectory, psr.Project.FullPath);

                        var lineNumberFileReference =
                            $"../blob/{branch}/{relativePath.Replace("\\", "/")}#L{psr.Project.TfmLineNumber}"
                                .EscapeUriString();
                        var name = relativePath.ShrinkPath("...");

                        return new MarkdownCheckListItem(false, $"[{name}]({lineNumberFileReference})");
                    })));

            document.AppendParagraph(
                "Consider upgrading projects to either the current release, or the nearest LTS TFM version.");

            document.AppendParagraph(
                $"If any of these projects are intentionally targeting an unsupported version, " +
                $"you can optionally configure to ignore including them in this automated issue. " +
                $"Create a file at the root of the repository, named *dotnet-versionsweeper.json* and " +
                $"add an `ignore` entry following the " +
                $"[globbing patterns detailed here](https://docs.microsoft.com/dotnet/api/microsoft.extensions.filesystemglobbing.matcher#remarks).");

            document.AppendCode("json", @"{
    ""ignore"": [
        ""**/path/to/example.csproj""
    ]
}");
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
                var name = relativePath.ShrinkPath("...");

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
        ""**/path/to/example.csproj""
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
