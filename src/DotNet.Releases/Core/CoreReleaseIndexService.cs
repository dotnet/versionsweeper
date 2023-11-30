// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

public sealed class CoreReleaseIndexService(IMemoryCache cache) : ICoreReleaseIndexService
{
    const string NetCoreKey = nameof(NetCoreKey);

    Task<ReadOnlyDictionary<Product, ReadOnlyCollection<ProductRelease>>?>
        ICoreReleaseIndexService.GetReleasesAsync() =>
        cache.GetOrCreateAsync(
            NetCoreKey,
            static async entry =>
            {
                ProductCollection products = await ProductCollection.GetAsync();

                var map = new ConcurrentDictionary<Product, ReadOnlyCollection<ProductRelease>>();
                await Parallel.ForEachAsync(products, async (product, token) =>
                {
                    map[product] = await product.GetReleasesAsync();
                });

                return map.AsReadOnly();
            });
}
