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
        foreach (string releaseName in _indexService.FrameworkReseaseFileNames)
        {
            FrameworkRelease? frameworkRelease =
                await _cache.GetOrCreateAsync(
                    releaseName,
                    async entry =>
                    {
                        string? name = entry.Key.ToString();
                        string resourceName = $"DotNet.Releases.Data.{name}";
                        using Stream? stream = _executingAssembly.GetManifestResourceStream(resourceName);
                        using StreamReader reader = new(stream!);

                        string json = await reader.ReadToEndAsync();

                        return json.FromJson<FrameworkRelease>(new());
                    });

            yield return frameworkRelease!;
        }
    }
}
