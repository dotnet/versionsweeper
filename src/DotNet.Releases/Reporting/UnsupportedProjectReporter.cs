﻿using DotNet.Extensions;
using DotNet.Models;
using DotNet.Releases.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        async IAsyncEnumerable<ProjectSupportReport> IUnsupportedProjectReporter.ReportAsync(Project project)
        {
            HashSet<TargetFrameworkMonikerSupport> resultingSupports = new();

            await foreach (var (index, coreRelease)
                in _coreReleaseService.GetAllReleasesAsync())
            {
                var tfmSupports =
                    project.Tfms.Select(
                        tfm => TryEvaluateReleaseSupport(
                            tfm, coreRelease.ChannelVersion,
                            index, out var tfmSupport)
                                ? tfmSupport : null)
                        .Where(tfmSupport => tfmSupport is not null);

                if (tfmSupports.Any())
                {
                    var supports = await Task.WhenAll(
                        tfmSupports.Where(support => support?.IsUnsupported ?? false)
                            .Select(
                            async support =>
                            {
                                var release = await _coreReleaseIndexService.GetNextLtsVersionAsync(
                                    coreRelease.LatestRelease);

                                return support! with { NearestLtsVersion = release!.TargetFrameworkMoniker };
                            }));

                    foreach (var support in supports)
                    {
                        resultingSupports.Add(support);
                    }
                }
            }

            await foreach (var frameworkRelease
                in _frameworkReleaseService.GetAllReleasesAsync())
            {
                var tfmSupports =
                    project.Tfms.Select(
                        tfm => TryEvaluateReleaseSupport(
                            tfm, frameworkRelease!.Version,
                            frameworkRelease, out var tfmSupport)
                                ? tfmSupport : null)
                        .Where(tfmSupport => tfmSupport is not null);

                if (tfmSupports.Any())
                {
                    var supports = await Task.WhenAll(
                        tfmSupports.Where(support => support?.IsUnsupported ?? false)
                        .Select(
                            async support =>
                            {
                                var release = await _frameworkReleaseService.GetNextLtsVersionAsync(
                                    frameworkRelease.Version.AsSemanticVersion());

                                return support! with
                                {
                                    NearestLtsVersion = release!.TargetFrameworkMoniker
                                };
                            }));

                    foreach (var support in supports)
                    {
                        resultingSupports.Add(support);
                    }
                }
            }

            if (resultingSupports.Any())
                yield return new(project, resultingSupports);
        }

        static bool TryEvaluateReleaseSupport(
            string tfm, string version,
            IRelease release,
            out TargetFrameworkMonikerSupport? tfmSupport)
        {
            if (TargetFrameworkMonikerMap.RawMapsToKnown(tfm, release.TargetFrameworkMoniker))
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
