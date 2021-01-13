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

        public async IAsyncEnumerable<ReleaseDetails?> GetAllReleasesAsync()
        {
            var releases = await _indexService.GetReleaesAsync();
            foreach (var release in releases?.ReleasesIndex ?? Enumerable.Empty<ReleasesIndex>())
            {
                var releaseDetails =
                    await _cache.GetOrCreateAsync(
                        release.ReleasesJson,
                        async _ =>
                        {
                            var releaseJson = await _httpClient.GetStringAsync(release.ReleasesJson);
                            return releaseJson.FromJson<ReleaseDetails>();
                        });

                yield return releaseDetails;
            }
        }
    }
}
