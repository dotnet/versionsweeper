// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.VersionSweeper;
using Xunit;

namespace DotNet.VersionSweeperTests;

public sealed class StringExtensionsTests
{
    public static IEnumerable<object[]> AsMaskedExtensionsInput = new[]
    {
            new object[] { "*.csproj", new string[] { "*.csproj" } },
            ["*.csproj|*.fsproj", new string[] { "*.csproj", "*.fsproj" }],
            ["*.csproj,*.fsproj", new string[] { "*.csproj", "*.fsproj" }],
            ["*.vbproj;*.csproj", new string[] { "*.vbproj", "*.csproj" }],
            ["*.csproj;", new string[] { "*.csproj" }],
            ["", Array.Empty<string>()]
        };

    [
        Theory,
        MemberData(nameof(AsMaskedExtensionsInput))
    ]
    public void AsMaskedExtensionsTest(string pattern, string[] expected) =>
        Assert.Equal(expected, pattern.SplitOnExpectedDelimiters());
}
