using Xunit;
using System;
using System.Collections.Generic;
using DotNet.VersionSweeper;

namespace DotNet.VersionSweeperTests
{
    public class StringExtensionsTests
    {
        public static IEnumerable<object[]> AsMaskedExtensionsInput = new[]
        {
            new object[] { "*.csproj", new string[] { "*.csproj" } },
            new object[] { "*.csproj|*.fsproj", new string[] { "*.csproj", "*.fsproj" } },
            new object[] { "*.csproj,*.fsproj", new string[] { "*.csproj", "*.fsproj" } },
            new object[] { "*.vbproj;*.csproj", new string[] { "*.vbproj", "*.csproj" } },
            new object[] { "*.csproj;", new string[] { "*.csproj" } },
            new object[] { "", Array.Empty<string>() }
        };

        [
            Theory,
            MemberData(nameof(AsMaskedExtensionsInput))
        ]
        public void AsMaskedExtensionsTest(string pattern, string[] expected) =>
            Assert.Equal(expected, pattern.SplitOnExpectedDelimiters());
    }
}