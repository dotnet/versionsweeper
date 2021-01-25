using System.Collections.Generic;

namespace DotNet.Releases
{
    public interface IFrameworkReleaseIndexService
    {
        HashSet<string> FrameworkReseaseFileNames { get; }
    }
}
