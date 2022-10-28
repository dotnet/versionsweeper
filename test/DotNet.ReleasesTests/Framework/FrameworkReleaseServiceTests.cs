// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.Releases;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNet.ReleasesTests;

public sealed class FrameworkReleaseServiceTests
{
    readonly IFrameworkReleaseIndexService _indexService = new FrameworkReleaseIndexService();
    readonly MemoryCache _cache = new(Options.Create(new MemoryCacheOptions()));

    [
        Theory,
        InlineData("3.0", "3.5.0-sp1"),
        InlineData("3.5", "3.5.0-sp1"),
        InlineData("4.0", "4.8"),
        InlineData("4.5", "4.8"),
        InlineData("4.5.2", "4.8"),
        InlineData("4.6.1", "4.8"),
        InlineData("4.7", "4.8"),
        InlineData("4.7.2", "4.8"),
        InlineData("4.6", "4.8")
    ]
    public async Task GetNextLtsVersionAsyncTest(
        string releaseVersion, string expected)
    {
        IFrameworkReleaseService service =
            new FrameworkReleaseService(_indexService, _cache);

        var actual = await service.GetNextLtsVersionAsync(releaseVersion);
        Assert.Equal(expected, actual.Version);
    }
}
