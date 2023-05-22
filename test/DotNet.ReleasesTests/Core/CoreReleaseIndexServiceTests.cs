// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.Releases;
using Microsoft.Deployment.DotNet.Releases;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNet.ReleasesTests;

public sealed class CoreReleaseIndexServiceTests
{
    readonly MemoryCache _cache = new(Options.Create(new MemoryCacheOptions()));

    [
        Theory,
        InlineData("1.1.0", "6.0"),
        InlineData("1.0.0", "6.0"),
        InlineData("2.2.8", "6.0"),
        InlineData("3.0.3", "6.0"),
        InlineData("3.1.11", "6.0"),
        InlineData("5.0.17", "6.0")
    ]
    public async Task GetNextLtsVersionAsyncTest(
        string releaseVersion, string expected)
    {
        ICoreReleaseIndexService service = new CoreReleaseIndexService(_cache);

        Product actual = await service.GetNextLtsVersionAsync(releaseVersion);
        Assert.Equal(expected, actual.ProductVersion);
    }
}
