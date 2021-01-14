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

        public Task<CoreReleases?> GetReleaesAsync() =>
            _cache.GetOrCreateAsync(
                ReleaseIndex,
                async entry =>
                {
                    var coreReleasesJson = await _httpClient.GetStringAsync(entry.Key.ToString());
                    return coreReleasesJson.FromJson<CoreReleases>();
                });
    }
}
