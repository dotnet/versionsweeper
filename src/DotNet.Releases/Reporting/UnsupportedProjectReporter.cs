// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

internal sealed class UnsupportedProjectReporter(
    ICoreReleaseIndexService coreReleaseIndexService,
    IFrameworkReleaseService frameworkReleaseService)
    : UnsupportedReporterBase, IUnsupportedProjectReporter
{
    async IAsyncEnumerable<ProjectSupportReport> IUnsupportedProjectReporter.ReportAsync(
        Project project, int outOfSupportWithinDays)
    {
        HashSet<TargetFrameworkMonikerSupport> resultingSupports = [];
        DateTime outOfSupportWithinDate = DateTimeOffset.UtcNow.Date.AddDays(outOfSupportWithinDays);

        var products = await coreReleaseIndexService.GetReleasesAsync();
        foreach (var product in products?.Keys ?? Enumerable.Empty<Product>())
        {
            IEnumerable<TargetFrameworkMonikerSupport?> tfmSupports =
                project.Tfms.Select(
                    tfm => TryEvaluateDotNetSupport(
                        tfm, product.ProductVersion,
                        product, outOfSupportWithinDate, out TargetFrameworkMonikerSupport? tfmSupport)
                            ? tfmSupport : null)
                    .Where(tfmSupport => tfmSupport is not null);

            if (tfmSupports.Any())
            {
                TargetFrameworkMonikerSupport[] supports = await Task.WhenAll(
                    tfmSupports.Where(support => support?.IsUnsupported ?? false)
                        .Select(
                        async support =>
                        {
                            var release = await coreReleaseIndexService.GetNextLtsVersionAsync(
                                product.LatestReleaseVersion.ToString());

                            return support! with
                            {
                                NearestLtsVersion = release!.GetTargetFrameworkMoniker()
                            };
                        }));

                foreach (TargetFrameworkMonikerSupport? support in supports)
                {
                    resultingSupports.Add(support);
                }
            }
        }

        await foreach (var frameworkRelease
            in frameworkReleaseService.GetAllReleasesAsync())
        {
            IEnumerable<TargetFrameworkMonikerSupport?> tfmSupports =
                project.Tfms.Select(
                    tfm => TryEvaluateDotNetFrameworkSupport(
                        tfm, frameworkRelease!.Version,
                        frameworkRelease, outOfSupportWithinDate, out TargetFrameworkMonikerSupport? tfmSupport)
                            ? tfmSupport : null)
                    .Where(tfmSupport => tfmSupport is not null);

            if (tfmSupports.Any())
            {
                TargetFrameworkMonikerSupport[] supports = await Task.WhenAll(
                    tfmSupports.Where(support => support?.IsUnsupported ?? false)
                    .Select(
                        async support =>
                        {
                            var release = await frameworkReleaseService.GetNextLtsVersionAsync(
                                (LabeledVersion)frameworkRelease.Version);

                            return support! with
                            {
                                NearestLtsVersion = release!.TargetFrameworkMoniker
                            };
                        }));

                foreach (TargetFrameworkMonikerSupport? support in supports)
                {
                    resultingSupports.Add(support);
                }
            }
        }

        if (resultingSupports.Count != 0)
            yield return new(project, resultingSupports);
    }
}
