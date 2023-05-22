// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

internal sealed class UnsupportedDockerfileReporter : UnsupportedReporterBase, IUnsupportedDockerfileReporter
{
    readonly ICoreReleaseIndexService _coreReleaseIndexService;
    readonly IFrameworkReleaseService _frameworkReleaseService;

    public UnsupportedDockerfileReporter(
        ICoreReleaseIndexService coreReleaseIndexService,
        IFrameworkReleaseService frameworkReleaseService) =>
        (_coreReleaseIndexService, _frameworkReleaseService) =
            (coreReleaseIndexService, frameworkReleaseService);

    async IAsyncEnumerable<DockerfileSupportReport> IUnsupportedDockerfileReporter.ReportAsync(
        Dockerfile dockerfile, int outOfSupportWithinDays)
    {
        HashSet<TargetFrameworkMonikerSupport> resultingSupports = new();
        DateTime outOfSupportWithinDate = DateTimeOffset.UtcNow.Date.AddDays(outOfSupportWithinDays);

        System.Collections.ObjectModel.ReadOnlyDictionary<Product, IReadOnlyCollection<ProductRelease>>? products = await _coreReleaseIndexService.GetReleasesAsync();
        foreach (Product product in products?.Keys ?? Enumerable.Empty<Product>())
        {
            IEnumerable<TargetFrameworkMonikerSupport?> tfmSupports =
                dockerfile.ImageDetails!.Select(
                    details => TryEvaluateDotNetSupport(
                        details.TargetFrameworkMoniker, product.ProductVersion,
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
                            Product? release = await _coreReleaseIndexService.GetNextLtsVersionAsync(
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

        await foreach (FrameworkRelease frameworkRelease
            in _frameworkReleaseService.GetAllReleasesAsync())
        {
            IEnumerable<TargetFrameworkMonikerSupport?> tfmSupports =
                dockerfile.ImageDetails!.Select(
                    details => TryEvaluateDotNetFrameworkSupport(
                        details.TargetFrameworkMoniker, frameworkRelease!.Version,
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
                            FrameworkRelease? release = await _frameworkReleaseService.GetNextLtsVersionAsync(
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

        if (resultingSupports.Any())
            yield return new(dockerfile, resultingSupports);
    }
}
