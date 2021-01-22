using DotNet.Extensions;
using DotNet.Models;
using DotNet.Versions.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Versions
{
    public interface IFrameworkReleaseService
    {
        IAsyncEnumerable<FrameworkRelease?> GetAllReleasesAsync();

        async ValueTask<FrameworkRelease?> GetNextLtsVersionAsync(string releaseVersion)
        {
            var version = releaseVersion.AsSemanticVersion();
            var sequence = GetAllReleasesAsync();
            var releases = await sequence.ToListAsync();

            return releases?
                .Where(release => release is not null)
                .Select(release => (Version: release!.SemanticVersion.AsSemanticVersion(), Release: release!))
                .Where(_ =>
                    _.Version > version &&
                    _.Release.SupportPhase.IsSupported(_.Release.EndOfLifeDate.GetValueOrDefault()))
                .OrderBy(_ => _.Version)
                .Select(_ => _.Release)
                .FirstOrDefault();
        }
    }
}
