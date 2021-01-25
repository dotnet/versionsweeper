using DotNet.Extensions;
using DotNet.Models;
using DotNet.Releases.Extensions;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Releases
{
    public interface IFrameworkReleaseService
    {
        IAsyncEnumerable<FrameworkRelease> GetAllReleasesAsync();

        async ValueTask<FrameworkRelease?> GetNextLtsVersionAsync(SemanticVersion releaseVersion)
        {
            var sequence = GetAllReleasesAsync();
            var releases = await sequence.ToListAsync();

            return releases?
                .Where(release => release is not null)
                .Select(release =>
                    (Version: release!.Version.AsSemanticVersion(), Release: release!))
                .Where(_ =>
                    _.Version > releaseVersion &&
                    _.Release.SupportPhase.IsSupported(
                        _.Release.EndOfLifeDate.GetValueOrDefault()))
                .OrderBy(_ => _.Version)
                .Select(_ => _.Release)
                .FirstOrDefault();
        }
    }
}
