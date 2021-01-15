using DotNet.Extensions;
using DotNet.Versions.Extensions;
using DotNet.Versions.Records;
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
                        tfm => TryEvaluateCoreSupport(
                            tfm, coreRelease, out var tfmSupport) ? tfmSupport : null)
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
                        tfm => TryEvaluateFrameworkSupport(
                            tfm, frameworkRelease, out var tfmSupport) ? tfmSupport : null)
                        .Where(tfmSupport => tfmSupport is not null);

                if (tfmSupports.Any())
                {
                    yield return new(projectPath, tfmSupports.ToHashSet()!);
                }
            }
        }

        static bool TryEvaluateCoreSupport(
            string tfm, ReleasesIndex? release, out TargetFrameworkMonikerSupport? tfmSupport)
        {
            if (release?.TargetFrameworkMoniker == tfm)
            {
                var isSupported = release.SupportPhase.IsSupported(release!.EolDate.ToDateTime());
                tfmSupport = new(tfm, release.ChannelVersion, !isSupported);
                return true;
            }

            tfmSupport = default;
            return false;
        }

        static bool TryEvaluateFrameworkSupport(
            string tfm, FrameworkRelease? release, out TargetFrameworkMonikerSupport? tfmSupport)
        {
            if (release?.TargetFrameworkMoniker == tfm)
            {
                var isSupported = release.SupportPhase.IsSupported(release!.EndOfLife.ToDateTime());
                tfmSupport = new(tfm, release.Version, !isSupported);
                return true;
            }

            tfmSupport = default;
            return false;
        }
    }
}
