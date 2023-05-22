// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

internal class UnsupportedReporterBase
{
    protected static bool TryEvaluateDotNetSupport(
        string tfm,
        string version,
        Product product,
        DateTime outOfSupportWithinDate,
        out TargetFrameworkMonikerSupport? tfmSupport)
    {
        IRelease release = ReleaseFactory.Create(
            product,
            pr => $"{pr.ProductName} {pr.ProductVersion}",
            product.GetTargetFrameworkMoniker(),
            product.SupportPhase,
            product.EndOfLifeDate,
            product.ReleasesJson.ToString());

        if (TargetFrameworkMonikerMap.RawMapsToKnown(tfm, release.TargetFrameworkMoniker))
        {
            bool isOutOfSupport = product.IsOutOfSupport() ||
                product.EndOfLifeDate <= outOfSupportWithinDate;

            tfmSupport = new(tfm, version, isOutOfSupport, release);
            return true;
        }

        tfmSupport = default;
        return false;
    }

    protected static bool TryEvaluateDotNetFrameworkSupport(
        string tfm, string version,
        IRelease release,
        DateTime outOfSupportWithinDate,
        out TargetFrameworkMonikerSupport? tfmSupport)
    {
        if (TargetFrameworkMonikerMap.RawMapsToKnown(tfm, release.TargetFrameworkMoniker))
        {
            bool isOutOfSupport = release.SupportPhase == SupportPhase.EOL ||
                release.EndOfLifeDate?.Date <= outOfSupportWithinDate;

            tfmSupport = new(tfm, version, isOutOfSupport, release);
            return true;
        }

        tfmSupport = default;
        return false;
    }
}
