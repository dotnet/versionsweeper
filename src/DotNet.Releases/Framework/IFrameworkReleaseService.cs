using DotNet.Models;
using System.Collections.Generic;

namespace DotNet.Versions
{
    public interface IFrameworkReleaseService
    {
        IAsyncEnumerable<FrameworkRelease?> GetAllReleasesAsync();
    }
}
