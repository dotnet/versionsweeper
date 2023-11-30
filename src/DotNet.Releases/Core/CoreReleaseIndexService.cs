// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;

namespace DotNet.Releases;

public sealed class CoreReleaseIndexService(IMemoryCache cache) : ICoreReleaseIndexService
{
    const string NetCoreKey = nameof(NetCoreKey);

    Task<IReadOnlyDictionary<Product, ReadOnlyCollection<ProductRelease>>?>
        ICoreReleaseIndexService.GetReleasesAsync() =>
        cache.GetOrCreateAsync(
            NetCoreKey,
            async entry =>
            {
                ProductCollection products = await ProductCollection.GetAsync();

                ConcurrentDictionary<Product, ReadOnlyCollection<ProductRelease>> map = new();
                await Parallel.ForEachAsync(products, async (product, token) =>
                {
                    if (product is null)
                    {
                        return;
                    }

                    var releases = await product.GetReleasesAsync();
                    if (releases is null)
                    {
                        return;
                    }

                    map[product] = releases;
                });

                return map.AsReadOnlyDictionary();
            });
}
