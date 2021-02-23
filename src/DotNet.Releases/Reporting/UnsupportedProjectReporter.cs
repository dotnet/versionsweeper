using DotNet.Models;
using DotNet.Releases.Extensions;
using Microsoft.Deployment.DotNet.Releases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Releases
{
    internal class UnsupportedProjectReporter : IUnsupportedProjectReporter
    {
        readonly ICoreReleaseIndexService _coreReleaseIndexService;
        readonly IFrameworkReleaseService _frameworkReleaseService;

        public UnsupportedProjectReporter(
            ICoreReleaseIndexService coreReleaseIndexService,
            IFrameworkReleaseService frameworkReleaseService) =>
            (_coreReleaseIndexService, _frameworkReleaseService) =
                (coreReleaseIndexService, frameworkReleaseService);

        async IAsyncEnumerable<ProjectSupportReport> IUnsupportedProjectReporter.ReportAsync(Project project)
        {
            HashSet<TargetFrameworkMonikerSupport> resultingSupports = new();

            var products = await _coreReleaseIndexService.GetReleasesAsync();
            foreach (var (product, release) 
                in products.SelectMany(p => p.Value, (kvp, releases) => (kvp.Key, releases)))
            {
                var tfmSupports =
                    project.Tfms.Select(
                        tfm => TryEvaluateReleaseSupport(
                            tfm, product.ProductVersion,
                            release, out var tfmSupport)
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
            string tfm, string version,
            ProductRelease productRelease,
            out TargetFrameworkMonikerSupport? tfmSupport)
        {
            var product = productRelease.Product;
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
                var isOutOfSupport = product.IsOutOfSupport();
                tfmSupport = new(tfm, version, isOutOfSupport, release);
                return true;
            }

            tfmSupport = default;
            return false;
        }

        static bool TryEvaluateReleaseSupport(
            string tfm, string version,
            IRelease release,
            out TargetFrameworkMonikerSupport? tfmSupport)
        {
            if (TargetFrameworkMonikerMap.RawMapsToKnown(tfm, release.TargetFrameworkMoniker))
            {
                var isOutOfSupport = release.SupportPhase == SupportPhase.EOL ||
                    release.EndOfLifeDate?.Date <= DateTime.Now.Date;
                tfmSupport = new(tfm, version, isOutOfSupport, release);
                return true;
            }

            tfmSupport = default;
            return false;
        }
    }
}
