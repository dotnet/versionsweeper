// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.Releases;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNet.ReleasesTests;

public sealed class CoreReleaseIndexServiceTests
{
    readonly MemoryCache _cache = new(Options.Create(new MemoryCacheOptions()));

    [
        Theory,
        InlineData("1.1.0", "8.0"),
        InlineData("1.0.0", "8.0"),
        InlineData("2.2.8", "8.0"),
        InlineData("3.0.3", "8.0"),
        InlineData("3.1.11", "8.0")
    ]
    public async Task GetNextLtsVersionAsyncTest(
        string releaseVersion, string expectedVersion)
    {
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
        ICoreReleaseIndexService service = new CoreReleaseIndexService(_cache);
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

        var result = await service.GetNextLtsVersionAsync(releaseVersion);
        Assert.Equal(expectedVersion, result.ProductVersion);
    }
}
