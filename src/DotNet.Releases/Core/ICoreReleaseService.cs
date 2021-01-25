using DotNet.Models;
using System.Collections.Generic;

namespace DotNet.Releases
{
    public interface ICoreReleaseService
    {
        IAsyncEnumerable<(ReleaseIndex Index, CoreReleaseDetails Details)> GetAllReleasesAsync();
    }
}
