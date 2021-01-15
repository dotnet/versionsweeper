using DotNet.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNet.Versions
{
    internal class CoreReleaseIndexService : ICoreReleaseIndexService
    {
        const string ReleaseIndex =
            "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/releases-index.json";

        readonly HttpClient _httpClient;
        readonly IMemoryCache _cache;

        public CoreReleaseIndexService(
            HttpClient httpClient, IMemoryCache cache) =>
            (_httpClient, _cache) = (httpClient, cache);

        Task<CoreReleases?> ICoreReleaseIndexService.GetReleaesAsync() =>
            _cache.GetOrCreateAsync(
                ReleaseIndex,
                async entry =>
                {
                    var coreReleasesJson = await _httpClient.GetStringAsync(entry.Key.ToString());
                    return coreReleasesJson.FromJson<CoreReleases>();
                });
    }
}
