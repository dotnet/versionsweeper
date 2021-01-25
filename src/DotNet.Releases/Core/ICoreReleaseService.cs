using DotNet.Models;
using System.Collections.Generic;

namespace DotNet.Releases
{
    public interface ICoreReleaseService
    {
        IAsyncEnumerable<(ReleasesIndex Index, CoreReleaseDetails Details)> GetAllReleasesAsync();
    }
}
