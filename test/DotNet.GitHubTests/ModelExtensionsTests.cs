// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.GitHub;
using DotNet.IO;
using DotNet.Models;
using DotNet.Releases;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNet.GitHubTests;

public sealed class ModelExtensionsTests
{
    readonly MemoryCache _cache = new(Options.Create(new MemoryCacheOptions()));

    [Fact]
    public async Task ToMarkDownCorrectlyGeneratesContentTest()
    {
        string projectPath = "test.csproj";

        try
        {
            await File.WriteAllTextAsync(projectPath, Constants.TestProjectXml);

            IProjectFileReader reader = new ProjectFileReader();
            Project project = await reader.ReadProjectAsync(projectPath);
            ICoreReleaseIndexService coreService = new CoreReleaseIndexService(_cache);
            IFrameworkReleaseService frameworkService =
                new FrameworkReleaseService(new FrameworkReleaseIndexService(), _cache);

            IUnsupportedProjectReporter reporter =
                new UnsupportedProjectReporter(coreService, frameworkService);

            await foreach (ProjectSupportReport report in reporter.ReportAsync(project, 90))
            {
                string expected = """
                    The following project file(s) target a .NET version which is no longer supported.
                    This is an auto-generated issue, detailed and discussed in [dotnet/docs#22271](https://github.com/dotnet/docs/issues/22271).

                    | Target version | End of life | Release notes | Nearest LTS TFM version |
                    | --- | --- | --- | --- |
                    | `net5.0` | May, 10 2022 | [net5.0 release notes](https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/5.0/releases.json) | `net6.0` |

                      - [ ] <a href='https://github.com/dotnet/docs/blob/main/test.csproj#L4' title='test.csproj'>test.csproj</a>

                    Consider upgrading projects to either the Standard Term Support (STS) or Long Term Support (LTS) versions.

                    If any of these projects listed in this issue are intentionally targeting an unsupported version,
                    you can optionally configure to ignore these results in future automation executions.
                    Create a (or update the) *dotnet-versionsweeper.json* file at the root of the repository and
                    add an `ignore` entry following the
                    [globbing patterns detailed here](https://learn.microsoft.com/dotnet/core/extensions/file-globbing).

                    ```json
                    {
                        "ignore": [
                            "**/path/to/example.csproj"
                        ]
                    }
                    ```

                    """
                    .ReplaceLineEndings();

                string actual = new HashSet<ProjectSupportReport>                
                    {
                        report
                    }
                    .ToMarkdownBody(
                        tfm: "net5.0",
                        options: new TestOptions(
                            Path.GetDirectoryName(
                                Path.GetFullPath(projectPath))))
                    .ReplaceLineEndings();
                
                Assert.Equal(expected, actual);
            }
        }
        finally
        {
            File.Delete(projectPath);
        }
    }
}

internal class TestOptions(string directory) : IRepoOptions
{
    public string Owner => "dotnet";
    public string Name => "docs";
    public string Branch => "main";
    public string Directory { get; } = directory;
}
