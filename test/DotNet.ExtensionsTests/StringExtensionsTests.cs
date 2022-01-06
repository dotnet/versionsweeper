// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace DotNet.Extensions.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void FirstAndLastSegementsOfPathTest()
    {
        var d = Path.DirectorySeparatorChar;
        foreach (var (input, expected, limit)
            in new[]
            {
                    (null, null, 10),
                    ($"path-project.csproj", "path-project.csproj", 20),
                    ($"example{d}of{d}some{d}path{d}project.csproj", "example/of/some/path/project.csproj", 40),
                    (
                        $"example{d}for{d}some{d}path-to-a{d}project.csproj",
                        "example/.../some/path-to-a/project.csproj",
                        40
                    ),
                    (
                        $"dapine{d}source{d}repos{d}dotnet-api-docs{d}samples{d}snippets{d}xaml{d}VS_Snippets_Wpf{d}FrameNavigationUIVisibilitySnippets{d}XAML.csproj",
                        "dapine/.../dotnet-api-docs/samples/snippets/xaml/VS_Snippets_Wpf/FrameNavigationUIVisibilitySnippets/XAML.csproj",
                        113
                    )
            })
        {
            Assert.Equal(expected, input.ShrinkPath(limit: limit));
        }
    }
}
