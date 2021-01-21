using DotNet.Models;
using System.Collections.Generic;

namespace DotNet.Versions
{
    public interface ICoreReleaseService
    {
        IAsyncEnumerable<CoreReleaseDetails?> GetAllReleasesAsync();
    }
}
