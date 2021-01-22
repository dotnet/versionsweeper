using DotNet.Extensions;
using DotNet.Models;
using DotNet.Versions.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Versions
{
    public interface ICoreReleaseIndexService
    {
        Task<CoreReleases?> GetReleaesAsync();

        async ValueTask<ReleasesIndex?> GetNextLtsVersionAsync(string releaseVersion)
        {
            var version = releaseVersion.AsSemanticVersion();
            var releases = await GetReleaesAsync();

            return releases?.ReleasesIndex
                .Select(release => (Version: release.LatestRelease.AsSemanticVersion(), Release: release))
                .Where(_ =>
                    _.Version > version &&
                    _.Release.SupportPhase.IsSupported(_.Release.EndOfLifeDate.GetValueOrDefault()))
                .OrderBy(_ => _.Version)
                .Select(_ => _.Release)
                .FirstOrDefault();
        }
    }
}
