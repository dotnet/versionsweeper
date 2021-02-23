using Microsoft.Deployment.DotNet.Releases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Releases
{
    public interface ICoreReleaseIndexService
    {
        Task<IReadOnlyDictionary<Product, IReadOnlyCollection<ProductRelease>>> GetReleasesAsync();

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
}
