// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

public interface IFrameworkReleaseService
{
    IAsyncEnumerable<FrameworkRelease> GetAllReleasesAsync();

    async ValueTask<FrameworkRelease?> GetNextLtsVersionAsync(LabeledVersion releaseVersion)
    {
        IAsyncEnumerable<FrameworkRelease> sequence = GetAllReleasesAsync();
        List<FrameworkRelease> releases = await sequence.ToListAsync();

        static bool IsOutOfSupport(FrameworkRelease release) =>
            release.SupportPhase == SupportPhase.EOL || release.EndOfLifeDate?.Date <= DateTimeOffset.UtcNow.Date;

        IOrderedEnumerable<(LabeledVersion Version, FrameworkRelease Release)>? orderedReleases = releases?
            .Where(release => release is not null)
            .Select(release =>
                (Version: (LabeledVersion)release!.Version, Release: release!))
            .Where(_ =>
                _.Version > releaseVersion && !IsOutOfSupport(_.Release))
            .OrderBy(_ => _.Version);

        return orderedReleases?.Select(_ => _.Release)
            .FirstOrDefault();
    }
}
