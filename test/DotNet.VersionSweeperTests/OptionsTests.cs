// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.VersionSweeper;
using Xunit;

namespace DotNet.VersionSweeperTests;

public class OptionsTests
{
    public static IEnumerable<object[]> NewOptionsSearchPatternInput = new[]
{
            new object[] { "*.csproj|project.json", new[] { "**/*.csproj", "**/project.json" } },
            new object[] { "*.fsproj;*.vbproj", new[] { "**/*.fsproj", "**/*.vbproj" } },
            new object[] { "*.xproj", new[] { "**/*.xproj" } },
            new object[] { "", Array.Empty<string>() },
            new object[] { null, Array.Empty<string>() }
        };

    [
        Theory,
        MemberData(nameof(NewOptionsSearchPatternInput))
    ]
    public void OptionsSearchPatternIsCorrectlyParsed(
        string inputPattern, IEnumerable<string> expectedPatterns)
    {
        Options options = new()
        {
            SearchPattern = inputPattern
        };

        Assert.Equal(
            expectedPatterns,
            options.SearchPattern
                .SplitOnExpectedDelimiters()
                .AsRecursivePatterns());
    }
}
