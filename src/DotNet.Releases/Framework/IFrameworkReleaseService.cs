using DotNet.Models;
using DotNet.Releases.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Releases
{
    public interface IFrameworkReleaseService
    {
        IAsyncEnumerable<FrameworkRelease> GetAllReleasesAsync();

        async ValueTask<FrameworkRelease?> GetNextLtsVersionAsync(LabeledVersion releaseVersion)
        {
            var sequence = GetAllReleasesAsync();
            var releases = await sequence.ToListAsync();

            var orderedReleases = releases?
                .Where(release => release is not null)
                .Select(release =>
                    (Version: (LabeledVersion)release!.Version, Release: release!))
                .Where(_ =>
                    _.Version > releaseVersion &&
                    _.Release.SupportPhase.IsSupported(
                        _.Release.EndOfLifeDate.GetValueOrDefault()))
                .OrderBy(_ => _.Version);

            return orderedReleases?.Select(_ => _.Release)
                .FirstOrDefault();
        }
    }
}
