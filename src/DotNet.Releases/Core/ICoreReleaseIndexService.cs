// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

public interface ICoreReleaseIndexService
{
    Task<ReadOnlyDictionary<Product, ReadOnlyCollection<ProductRelease>>?> GetReleasesAsync();

    async ValueTask<Product?> GetNextLtsVersionAsync(string releaseVersion)
    {
        var version = new ReleaseVersion(releaseVersion);
        ReadOnlyDictionary<Product, ReadOnlyCollection<ProductRelease>>? products = await GetReleasesAsync();

        return products?.SelectMany(kvp => kvp.Value)
            .Where(release => release.Version > version && !release.Product.IsOutOfSupport())
            .OrderBy(_ => _.Version)
            .Select(_ => _.Product)
            .FirstOrDefault();
    }
}
