using DotNet.Versions.Records;
using System.Collections.Generic;

namespace DotNet.Versions
{
    public interface IFrameworkReleaseService
    {
        IAsyncEnumerable<FrameworkRelease?> GetAllReleasesAsync();
    }
}
