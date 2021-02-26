// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotNet.GitHubActions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace DotNet.VersionSweeper
{
    public static class OptionsExtensions
    {
        public static Matcher AsGlobMatcher(this Options options, string[] ignoreGlobs)
        {
            Matcher matcher = new();

            matcher.AddExcludePatterns(ignoreGlobs);
            if (options.SearchPattern is not null)
            {
                matcher.AddIncludePatterns(
                    options.SearchPattern
                        .SplitOnExpectedDelimiters()
                        .AsRecursivePatterns());
            }

            return matcher;
        }

        public static string[] SplitOnExpectedDelimiters(this string? searchPattern) =>
            searchPattern is null or { Length: 0 }
                ? Array.Empty<string>()
                : (searchPattern.Contains(';'), searchPattern.Contains('|'), searchPattern.Contains(',')) switch
                {
                    (true, _, _) => searchPattern.Split(';', StringSplitOptions.RemoveEmptyEntries),
                    (_, true, _) => searchPattern.Split('|', StringSplitOptions.RemoveEmptyEntries),
                    (_, _, true) => searchPattern.Split(',', StringSplitOptions.RemoveEmptyEntries),

                    _ => new string[] { searchPattern }
                };

        public static IEnumerable<string> AsRecursivePatterns(this string[] patterns) =>
            patterns.Select(pattern => $"**/{pattern}");

        public static HashSet<string> AsFileExtensions(this Options options) =>
            options.SearchPattern
                .SplitOnExpectedDelimiters()
                .Select(Path.GetExtension)
                .Where(ext => ext is not null)
                .Select(ext => ext!)
                .ToHashSet();

        public static DirectoryInfo ToDirectoryInfo(this Options options) => new(options.Directory);

        public static void WriteDebugInfo(this Options options, IJobService job)
        {
            job.SetCommandEcho(true);

            StringBuilder builder = new();
            builder.AppendLine($"repository owner: {options.Owner}");
            builder.AppendLine($"repository name: {options.Name}");
            builder.AppendLine($"current branch: {options.Branch}");
            builder.AppendLine($"root directory to search: {options.Directory}");
            var parsedPatterns = string.Join(", ",
                options.SearchPattern?.SplitOnExpectedDelimiters().AsRecursivePatterns() ?? Array.Empty<string>());
            builder.AppendLine($"parsed patterns: {parsedPatterns}");
            builder.AppendLine($"report non-SDK style projects: {options.ReportNonSdkStyleProjects}");

            job.Info(builder.ToString());
        }
    }
}
