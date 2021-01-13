using DotNet.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNet.Versions
{
    public class ReleaseIndexService
    {
        const string ReleaseIndex =
            "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/releases-index.json";

        readonly HttpClient _httpClient;
        readonly IMemoryCache _cache;

        public ReleaseIndexService(
            HttpClient httpClient, IMemoryCache cache) =>
            (_httpClient, _cache) = (httpClient, cache);

        public async Task<Releases?> GetReleaesAsync()
        {
            var releases = await _cache.GetOrCreateAsync(
                ReleaseIndex,
                async _ =>
                {
                    var releases = await _httpClient.GetStringAsync(ReleaseIndex);
                    return releases.FromJson<Releases>();
                });

            return releases;
        }
    }
}
