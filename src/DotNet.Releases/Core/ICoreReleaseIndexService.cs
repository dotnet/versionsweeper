using DotNet.Extensions;
using DotNet.Models;
using DotNet.Releases.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Releases
{
    public interface ICoreReleaseIndexService
    {
        Task<CoreReleases?> GetReleaesAsync();

        async ValueTask<ReleaseIndex?> GetNextLtsVersionAsync(string releaseVersion)
        {
            var version = (LabeledVersion)releaseVersion;
            var releases = await GetReleaesAsync();

            return releases?.ReleasesIndex
                .Select(release => (Version: (LabeledVersion)release.LatestRelease, Release: release))
                .Where(_ =>
                    _.Version > version &&
                    _.Release.SupportPhase.IsSupported(_.Release.EndOfLifeDate.GetValueOrDefault()))
                .OrderBy(_ => _.Version)
                .Select(_ => _.Release)
                .FirstOrDefault();
        }
    }
}
