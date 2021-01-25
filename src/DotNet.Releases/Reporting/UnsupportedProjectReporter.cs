using DotNet.Extensions;
using DotNet.Models;
using DotNet.Releases.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Releases
{
    internal class UnsupportedProjectReporter : IUnsupportedProjectReporter
    {
        readonly ICoreReleaseService _coreReleaseService;
        readonly ICoreReleaseIndexService _coreReleaseIndexService;
        readonly IFrameworkReleaseService _frameworkReleaseService;

        public UnsupportedProjectReporter(
            ICoreReleaseService coreReleaseService,
            ICoreReleaseIndexService coreReleaseIndexService,
            IFrameworkReleaseService frameworkReleaseService) =>
            (_coreReleaseService, _coreReleaseIndexService, _frameworkReleaseService) =
                (coreReleaseService, coreReleaseIndexService, frameworkReleaseService);

        async IAsyncEnumerable<ProjectSupportReport> IUnsupportedProjectReporter.ReportAsync(
            string projectPath, string[] tfms)
        {
            await foreach (var (index, coreRelease)
                in _coreReleaseService.GetAllReleasesAsync())
            {
                var tfmSupports =
                    tfms.Select(
                        tfm => TryEvaluateReleaseSupport(
                            tfm, coreRelease.ChannelVersion,
                            index, out var tfmSupport)
                                ? tfmSupport : null)
                        .Where(tfmSupport => tfmSupport is not null);

                if (tfmSupports.Any())
                {
                    foreach (var support in tfmSupports)
                    {
                        var release = await _coreReleaseIndexService.GetNextLtsVersionAsync(
                            coreRelease.LatestRelease);
                        if (support is not null && release is not null)
                        {
                            support.NearestLtsVersion = release.LatestRelease;
                        }
                    }

                    yield return new(projectPath, tfmSupports.ToHashSet()!);
                }
            }

            await foreach (var frameworkRelease in _frameworkReleaseService.GetAllReleasesAsync())
            {
                var tfmSupports =
                    tfms.Select(
                        tfm => TryEvaluateReleaseSupport(
                            tfm, frameworkRelease!.Version,
                            frameworkRelease, out var tfmSupport)
                                ? tfmSupport : null)
                        .Where(tfmSupport => tfmSupport is not null);

                if (tfmSupports.Any())
                {
                    foreach (var support in tfmSupports)
                    {
                        var release = await _frameworkReleaseService.GetNextLtsVersionAsync(
                            frameworkRelease.Version.AsSemanticVersion());
                        if (support is not null && release is not null)
                        {
                            support.NearestLtsVersion = release.Version;
                        }
                    }

                    yield return new(projectPath, tfmSupports.ToHashSet()!);
                }
            }
        }

        static bool TryEvaluateReleaseSupport(
            string tfm, string version,
            IRelease release,
            out TargetFrameworkMonikerSupport? tfmSupport)
        {
            if (release.TargetFrameworkMoniker == tfm)
            {
                var isSupported = release.SupportPhase.IsSupported(
                    release!.EndOfLifeDate ?? default);
                tfmSupport = new(tfm, version, !isSupported, release);
                return true;
            }

            tfmSupport = default;
            return false;
        }
    }
}
