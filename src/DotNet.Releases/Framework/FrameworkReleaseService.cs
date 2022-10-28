// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

internal sealed class FrameworkReleaseService : IFrameworkReleaseService
{
    readonly IMemoryCache _cache;
    readonly IFrameworkReleaseIndexService _indexService;
    readonly Assembly _executingAssembly;

    public FrameworkReleaseService(
        IFrameworkReleaseIndexService indexService,
        IMemoryCache cache) =>
        (_indexService, _cache, _executingAssembly) =
            (indexService, cache, Assembly.GetExecutingAssembly());

    async IAsyncEnumerable<FrameworkRelease> IFrameworkReleaseService.GetAllReleasesAsync()
    {
        foreach (var releaseName in _indexService.FrameworkReseaseFileNames)
        {
            var frameworkRelease =
                await _cache.GetOrCreateAsync(
                    releaseName,
                    async entry =>
                    {
                        var name = entry.Key.ToString();
                        var resourceName = $"DotNet.Releases.Data.{name}";
                        using var stream = _executingAssembly.GetManifestResourceStream(resourceName);
                        using StreamReader reader = new(stream!);

                        var json = await reader.ReadToEndAsync();

                        return json.FromJson<FrameworkRelease>(new());
                    });

            yield return frameworkRelease!;
        }
    }
}
