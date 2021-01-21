using DotNet.Extensions;
using DotNet.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotNet.Versions
{
    public class FrameworkReleaseService : IFrameworkReleaseService
    {
        readonly IMemoryCache _cache;
        readonly IFrameworkReleaseIndexService _indexService;
        readonly Assembly _executingAssembly;

        public FrameworkReleaseService(
            IFrameworkReleaseIndexService indexService,
            IMemoryCache cache) =>
            (_indexService, _cache, _executingAssembly) = (indexService, cache, Assembly.GetExecutingAssembly());

        async IAsyncEnumerable<FrameworkRelease?> IFrameworkReleaseService.GetAllReleasesAsync()
        {
            foreach (var releaseName in _indexService.FrameworkReseaseFileNames)
            {
                var frameworkRelease =
                    await _cache.GetOrCreateAsync(
                        releaseName,
                        async entry =>
                        {
                            var name = entry.Key.ToString();
                            var resourceName = $"DotNet.Versions.Data.{name}";
                            using var stream = _executingAssembly.GetManifestResourceStream(resourceName);
                            using StreamReader reader = new(stream!);

                            var json = await reader.ReadToEndAsync();

                            return json?.FromJson<FrameworkRelease>(new()) ?? default;
                        });

                yield return frameworkRelease;
            }
        }
    }
}