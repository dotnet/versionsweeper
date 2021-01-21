using DotNet.Models;
using DotNet.Versions.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Versions
{
    internal class UnsupportedProjectReporter : IUnsupportedProjectReporter
    {
        readonly ICoreReleaseIndexService _coreReleaseService;
        readonly IFrameworkReleaseService _frameworkReleaseService;

        public UnsupportedProjectReporter(
            ICoreReleaseIndexService coreReleaseService, IFrameworkReleaseService frameworkReleaseService) =>
            (_coreReleaseService, _frameworkReleaseService) = (coreReleaseService, frameworkReleaseService);

        async IAsyncEnumerable<ProjectSupportReport> IUnsupportedProjectReporter.ReportAsync(
            string projectPath, string[] tfms)
        {
            var coreReleases = await _coreReleaseService.GetReleaesAsync();
            foreach (var coreRelease in coreReleases?.ReleasesIndex ?? Enumerable.Empty<ReleasesIndex>())
            {
                var tfmSupports =
                    tfms.Select(
                        tfm => TryEvaluateReleaseSupport(
                            tfm, coreRelease.ChannelVersion, coreRelease, out var tfmSupport)
                                ? tfmSupport : null)
                        .Where(tfmSupport => tfmSupport is not null);

                if (tfmSupports.Any())
                {
                    yield return new(projectPath, tfmSupports.ToHashSet()!);
                }
            }

            await foreach (var frameworkRelease in _frameworkReleaseService.GetAllReleasesAsync())
            {
                var tfmSupports =
                    tfms.Select(
                        tfm => TryEvaluateReleaseSupport(
                            tfm, frameworkRelease!.Version, frameworkRelease, out var tfmSupport)
                                ? tfmSupport : null)
                        .Where(tfmSupport => tfmSupport is not null);

                if (tfmSupports.Any())
                {
                    yield return new(projectPath, tfmSupports.ToHashSet()!);
                }
            }
        }

        static bool TryEvaluateReleaseSupport(
            string tfm, string version, IRelease? release, out TargetFrameworkMonikerSupport? tfmSupport)
        {
            if (release?.TargetFrameworkMoniker == tfm)
            {
                var isSupported = release.SupportPhase.IsSupported(release!.EndOfLifeDate ?? default);
                tfmSupport = new(tfm, version, !isSupported, release);
                return true;
            }

            tfmSupport = default;
            return false;
        }
    }
}
