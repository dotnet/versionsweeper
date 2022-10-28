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
        var projectPath = "test.csproj";

        try
        {
            await File.WriteAllTextAsync(projectPath, Constants.TestProjectXml);

            IProjectFileReader reader = new ProjectFileReader();
            var project = await reader.ReadProjectAsync(projectPath);
            ICoreReleaseIndexService coreService = new CoreReleaseIndexService(_cache);
            IFrameworkReleaseService frameworkService =
                new FrameworkReleaseService(new FrameworkReleaseIndexService(), _cache);

            IUnsupportedProjectReporter reporter =
                new UnsupportedProjectReporter(coreService, frameworkService);

            await foreach (var report in reporter.ReportAsync(project, 90))
            {
                var actualMD = new HashSet<ProjectSupportReport> { report }.ToMarkdownBody("net5.0", new TestOptions());
                var expectedMD = "";

                Assert.Equal(expectedMD, actualMD);
            }
        }
        finally
        {
            File.Delete(projectPath);
        }
    }
}

internal class TestOptions : IRepoOptions
{
    public string Owner => "dotnet";
    public string Name => "docs";
    public string Branch => "main";
    public string Directory => "";
}
