using DotNet.Extensions;
using Microsoft.Deployment.DotNet.Releases;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet.Releases
{
    public class CoreReleaseIndexService : ICoreReleaseIndexService
    {
        const string NetCoreKey = nameof(NetCoreKey);

        readonly IMemoryCache _cache;

        public CoreReleaseIndexService(IMemoryCache cache) => _cache = cache;

        Task<IReadOnlyDictionary<Product, IReadOnlyCollection<ProductRelease>>>
            ICoreReleaseIndexService.GetReleasesAsync() =>
            _cache.GetOrCreateAsync(
                NetCoreKey,
                async entry =>
                {
                    var products = await ProductCollection.GetAsync();

                    var map = new Dictionary<Product, IReadOnlyCollection<ProductRelease>>();
                    foreach (var product in products)
                    {
                        map[product] = await product.GetReleasesAsync();
                    }

                    return map.AsReadOnly();
                });
    }
}
