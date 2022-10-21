// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.Models;
using Xunit;

namespace DotNet.ModelsTests;

public sealed class ProjectTests
{
    public static IEnumerable<object[]> NewProjectInput = new[]
    {
            new object[] { "netcoreapp1.1", "project-path.csproj", ".csproj", new[] { "netcoreapp1.1" } },
            new object[] { "net5.0;netstandard2.0", "some/path.vbproj", ".vbproj", new[] { "net5.0", "netstandard2.0" } },
            new object[] { ";;net46", "old/bits/sample.fsproj", ".fsproj", new[] { "net46" } },
            new object[] { null, "project.csproj", ".csproj", Array.Empty<string>() },
            new object[] { "", "project-path.csproj", ".csproj", Array.Empty<string>() },
            new object[] { "netcoreapp3.1", null, null, new[] { "netcoreapp3.1" } }
        };

    [
        Theory,
        MemberData(nameof(NewProjectInput))
    ]
    public void ProjectCorrectlyHandlesProperties(
        string actualTfms, string actualPath, string expectedExt, string[] expectedTfms)
    {
        Project project = new()
        {
            FullPath = actualPath,
            RawTargetFrameworkMonikers = actualTfms
        };

        Assert.Equal(expectedExt, project.Extension);
        Assert.Equal(expectedTfms, project.Tfms);
    }
}
