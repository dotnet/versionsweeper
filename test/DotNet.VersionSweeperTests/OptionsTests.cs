// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.VersionSweeper;
using Xunit;

namespace DotNet.VersionSweeperTests;

public sealed class OptionsTests
{
    public static IEnumerable<object[]> NewOptionsSearchPatternInput = new[]
    {
        new object[] { "*.csproj|project.json", new[] { "**/*.csproj", "**/project.json" } },
        ["*.fsproj;*.vbproj", new[] { "**/*.fsproj", "**/*.vbproj" }],
        ["*.xproj", new[] { "**/*.xproj" }],
        ["", Array.Empty<string>()],
        [null, Array.Empty<string>()]
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
