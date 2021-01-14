using DotNet.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DotNet.Versions
{
    public class ReleaseService
    {
        readonly HttpClient _httpClient;
        readonly IMemoryCache _cache;
        readonly ReleaseIndexService _indexService;

        public ReleaseService(
            HttpClient httpClient, IMemoryCache cache, ReleaseIndexService indexService) =>
            (_httpClient, _cache, _indexService) = (httpClient, cache, indexService);

        public async IAsyncEnumerable<CoreReleaseDetails?> GetAllReleasesAsync()
        {
            var releases = await _indexService.GetReleaesAsync();
            foreach (var release in releases?.ReleasesIndex ?? Enumerable.Empty<ReleasesIndex>())
            {
                var coreReleaseDetails =
                    await _cache.GetOrCreateAsync(
                        release.ReleasesJson,
                        async entry =>
                        {
                            var releaseJson = await _httpClient.GetStringAsync(entry.Key.ToString());
                            return releaseJson.FromJson<CoreReleaseDetails>();
                        });

                yield return coreReleaseDetails;
            }
        }
    }
}
