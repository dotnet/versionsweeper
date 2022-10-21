// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

internal class UnsupportedProjectReporter : IUnsupportedProjectReporter
{
    readonly ICoreReleaseIndexService _coreReleaseIndexService;
    readonly IFrameworkReleaseService _frameworkReleaseService;

    public UnsupportedProjectReporter(
        ICoreReleaseIndexService coreReleaseIndexService,
        IFrameworkReleaseService frameworkReleaseService) =>
        (_coreReleaseIndexService, _frameworkReleaseService) =
            (coreReleaseIndexService, frameworkReleaseService);

    async IAsyncEnumerable<ProjectSupportReport> IUnsupportedProjectReporter.ReportAsync(
        Project project, int outOfSupportWithinDays)
    {
        HashSet<TargetFrameworkMonikerSupport> resultingSupports = new();
        DateTime outOfSupportWithinDate = DateTimeOffset.UtcNow.Date.AddDays(outOfSupportWithinDays);

        var products = await _coreReleaseIndexService.GetReleasesAsync();
        foreach (var product in products.Keys)
        {
            var tfmSupports =
                project.Tfms.Select(
                    tfm => TryEvaluateReleaseSupport(
                        tfm, product.ProductVersion,
                        product, outOfSupportWithinDate, out var tfmSupport)
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
                                product.LatestReleaseVersion.ToString());

                            return support! with
                            {
                                NearestLtsVersion = release!.GetTargetFrameworkMoniker()
                            };
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
                        frameworkRelease, outOfSupportWithinDate, out var tfmSupport)
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
                                (LabeledVersion)frameworkRelease.Version);

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
        string tfm,
        string version,
        Product product,
        DateTime outOfSupportWithinDate,
        out TargetFrameworkMonikerSupport? tfmSupport)
    {
        var release = ReleaseFactory.Create(
            product,
            pr => $"{pr.ProductName} {pr.ProductVersion}",
            product.ProductName switch
            {
                ".NET" => $"net{product.ProductVersion}",
                ".NET Core" => $"netcoreapp{product.ProductVersion}",
                _ => product.ProductVersion
            },
            product.SupportPhase,
            product.EndOfLifeDate,
            product.ReleasesJson.ToString());

        if (TargetFrameworkMonikerMap.RawMapsToKnown(tfm, release.TargetFrameworkMoniker))
        {
            var isOutOfSupport = product.IsOutOfSupport() ||
                product.EndOfLifeDate <= outOfSupportWithinDate;

            tfmSupport = new(tfm, version, isOutOfSupport, release);
            return true;
        }

        tfmSupport = default;
        return false;
    }

    static bool TryEvaluateReleaseSupport(
        string tfm, string version,
        IRelease release,
        DateTime outOfSupportWithinDate,
        out TargetFrameworkMonikerSupport? tfmSupport)
    {
        if (TargetFrameworkMonikerMap.RawMapsToKnown(tfm, release.TargetFrameworkMoniker))
        {
            var isOutOfSupport = release.SupportPhase == SupportPhase.EOL ||
                release.EndOfLifeDate?.Date <= outOfSupportWithinDate;

            tfmSupport = new(tfm, version, isOutOfSupport, release);
            return true;
        }

        tfmSupport = default;
        return false;
    }
}
