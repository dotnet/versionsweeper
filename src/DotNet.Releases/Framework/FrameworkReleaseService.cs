// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

public sealed class FrameworkReleaseService(
    IFrameworkReleaseIndexService indexService,
    IMemoryCache cache) : IFrameworkReleaseService
{
    readonly Assembly _executingAssembly = Assembly.GetExecutingAssembly();

    async IAsyncEnumerable<FrameworkRelease> IFrameworkReleaseService.GetAllReleasesAsync()
    {
        foreach (var releaseName in indexService.FrameworkReleaseFileNames)
        {
            var frameworkRelease =
                await cache.GetOrCreateAsync(
                    releaseName,
                    async entry =>
                    {
                        string? name = entry.Key.ToString();
                        string resourceName = $"DotNet.Releases.Data.{name}";
                        using Stream? stream = _executingAssembly.GetManifestResourceStream(resourceName);
                        using StreamReader reader = new(stream!);

                        string json = await reader.ReadToEndAsync();

                        return json.FromJson<FrameworkRelease>(
                            ReleasesJsonSerializerContext.Default.FrameworkRelease);
                    });

            yield return frameworkRelease!;
        }
    }
}
