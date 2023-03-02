// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;

namespace DotNet.Releases;

public interface ICoreReleaseIndexService
{
    Task<ReadOnlyDictionary<Product, IReadOnlyCollection<ProductRelease>>?> GetReleasesAsync();

    async ValueTask<Product?> GetNextLtsVersionAsync(string releaseVersion)
    {
        var version = new ReleaseVersion(releaseVersion);
        var products = await GetReleasesAsync();

        return products?.SelectMany(kvp => kvp.Value)
            .Where(release => release.Version > version && !release.Product.IsOutOfSupport())
            .OrderBy(_ => _.Version)
            .Select(_ => _.Product)
            .FirstOrDefault();
    }
}
