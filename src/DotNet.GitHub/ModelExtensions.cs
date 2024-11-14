// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public static class ModelExtensions
{
    public static string ToMarkdownBody(
        this ISet<ProjectSupportReport> psrs,
        string tfm,
        IRepoOptions options)
    {
        (string rootDirectory, string branch) = (options.Directory, options.Branch);
        IMarkdownDocument document = new MarkdownDocument();

        document.AppendParagraph("""
            The following project file(s) target a .NET version that's no longer supported.
            This is an auto-generated issue, detailed and discussed in [dotnet/docs#22271](https://github.com/dotnet/docs/issues/22271).
            """);

        TargetFrameworkMonikerSupport tfmSupport =
            psrs.First()
                .TargetFrameworkMonikerSupports
                .First(tfms => tfms.TargetFrameworkMoniker == tfm);

        document.AppendTable(
            new MarkdownTableHeader(
                new("Target version"),
                new("End of life"),
                new("Release notes"),
                new("Nearest LTS TFM version")),
            [
                    new MarkdownTableRow(
                    $"`{tfmSupport.TargetFrameworkMoniker}`",
                    tfmSupport.Release.EndOfLifeDate.HasValue
                        ? $"{tfmSupport.Release.EndOfLifeDate:MMMM, dd yyyy}" : "N/A",
                    new MarkdownLink(
                        $"{tfmSupport.TargetFrameworkMoniker} release notes", tfmSupport.Release.ReleaseNotesUrl)
                        .ToString(),
                    $"`{tfmSupport.NearestLtsVersion}`")
            ]);

        document.AppendList(
            new MarkdownList(
                psrs.OrderBy(psr => psr.Project.FullPath).Select(psr =>
                {
                    string anchor = ToLineNumberUrl(
                        psr.Project.FullPath, psr.Project.TargetFrameworkMonikerLineNumber, options);
                    return new MarkdownCheckListItem(false, anchor);
                })));

        document.AppendParagraph(
            "Consider upgrading projects to either the Standard Term Support (STS) or Long Term Support (LTS) versions.");

        document.AppendParagraph($"""
            If any of these projects listed in this issue are intentionally targeting an unsupported version,
            you can optionally configure to ignore these results in future automation executions.
            Create a (or update the) *dotnet-versionsweeper.json* file at the root of the repository and
            add an `ignore` entry following the
            [globbing patterns detailed here](https://learn.microsoft.com/dotnet/core/extensions/file-globbing).
            """);

        document.AppendCode("json", """
        {
            "ignore": [
                "**/path/to/example.csproj"
            ]
        }
        """);

        return document.ToString();
    }

    public static string ToMarkdownBody(
        this ISet<DockerfileSupportReport> dfsr,
        string tfm,
        IRepoOptions options)
    {
        (string rootDirectory, string branch) = (options.Directory, options.Branch);
        IMarkdownDocument document = new MarkdownDocument();

        document.AppendParagraph("""
            The following Dockerfile(s) target a .NET version that's no longer supported.
            This is an auto-generated issue, detailed and discussed in [dotnet/docs#22271](https://github.com/dotnet/docs/issues/22271).
            """);

        TargetFrameworkMonikerSupport tfmSupport =
            dfsr.First()
                .TargetFrameworkMonikerSupports
                .First(tfms => tfms.TargetFrameworkMoniker == tfm);

        document.AppendTable(
            new MarkdownTableHeader(
                new("Target version"),
                new("End of life"),
                new("Release notes"),
                new("Nearest LTS TFM version")),
            [
                    new MarkdownTableRow(
                    $"`{tfmSupport.TargetFrameworkMoniker}`",
                    tfmSupport.Release.EndOfLifeDate.HasValue
                        ? $"{tfmSupport.Release.EndOfLifeDate:MMMM, dd yyyy}" : "N/A",
                    new MarkdownLink(
                        $"{tfmSupport.TargetFrameworkMoniker} release notes", tfmSupport.Release.ReleaseNotesUrl)
                        .ToString(),
                    $"`{tfmSupport.NearestLtsVersion}`")
            ]);

        document.AppendList(
            new MarkdownList(
                dfsr.SelectMany(sr => sr.TargetFrameworkMonikerSupports.Select(tfms => (sr.Dockerfile, tfms)))
                    .OrderBy(t => t.Dockerfile.FullPath)
                    .SelectMany(t =>
                    {
                        return t.Dockerfile.ImageDetails!.Select(i =>
                        {
                            string anchor = ToLineNumberUrl(t.Dockerfile.FullPath, i.LineNumber, options);
                            return new MarkdownCheckListItem(false, anchor);
                        });
                    })));

        document.AppendParagraph(
            "Consider upgrading Dockerfile images to either the Standard Term Support (STS) or Long Term Support (LTS) versions.");

        document.AppendParagraph($"""
            If any of these Dockerfile(s) listed in this issue are intentionally targeting an unsupported version,
            you can optionally configure to ignore these results in future automation executions.
            Create a (or update the) *dotnet-versionsweeper.json* file at the root of the repository and
            add an `ignore` entry following the
            [globbing patterns detailed here](https://learn.microsoft.com/dotnet/core/extensions/file-globbing).
            """);

        document.AppendCode("json", """
        {
            "ignore": [
                "**/path/to/Dockerfile"
            ]
        }
        """);

        return document.ToString();
    }

    private static string ToLineNumberUrl(
        string fullPath, int lineNumber, IRepoOptions options)
    {
        if (fullPath is null)
        {
            return "N/A";
        }

        string relativePath =
                Path.GetRelativePath(options.Directory, fullPath!)
                    .Replace("\\", "/");
        string lineNumberFileReference =
            $"https://github.com/{options.Owner}/{options.Name}/blob/{options.Branch}/{relativePath}#L{lineNumber}";
        string? name = relativePath.ShrinkPath("...");

        // Must force anchor link, as GitHub assumes site-relative links.
        return $"<a href='{lineNumberFileReference}' title='{name}'>{name}</a>";
    }

    public static bool TryCreateIssueContent(
        this IList<string> projectPaths,
        string rootDirectory,
        string branch,
        out (string Title, string MarkdownBody) result)
    {
        if (projectPaths is { Count: 0 })
        {
            result = (null!, null!);
            return false;
        }

        IMarkdownDocument document = new MarkdownDocument();

        document.AppendParagraph($"""
            There are {projectPaths.Count} project(s) using the non-SDK-style project format.
            This is an auto-generated issue, detailed and discussed in [dotnet/docs#22271](https://github.com/dotnet/docs/issues/22271).
            """);

        static MarkdownCheckListItem AsCheckListItem(
            string fullPath, string root, string branch)
        {
            string relativePath =
                Path.GetRelativePath(root, fullPath);

            string path = $"../blob/{branch}/{relativePath.Replace("\\", "/")}".EscapeUriString();
            string? name = relativePath.ShrinkPath("...");

            // Must force anchor link, as GitHub assumes site-relative links.
            string anchor = $"<a href='{path}' title='{name}'>{name}</a>";

            return new MarkdownCheckListItem(false, anchor);
        }

        document.AppendList(
            new MarkdownList(
                projectPaths
                    .Select(proj => AsCheckListItem(proj, rootDirectory, branch))));

        document.AppendParagraph(
            "Consider upgrading the project(s) to the [SDK-style format](https://docs.microsoft.com/dotnet/standard/frameworks).");

        document.AppendParagraph($"""
            If any of these projects listed in this issue are intentionally demonstrating the old project style,
            you can optionally configure to ignore these results in future automation executions.
            Create a (or update the) *dotnet-versionsweeper.json* file at the root of the repository and
            add an `ignore` entry following the
            [globbing patterns detailed here](https://learn.microsoft.com/dotnet/core/extensions/file-globbing).
            """);

        document.AppendCode("json", """
        {
            "ignore": [
                "**/path/to/example.csproj"
            ]
        }
        """);

        result =
            (
                Title: "Upgrade project(s) to the SDK-style project format",
                MarkdownBody: document.ToString()
            );

        return true;
    }
}
