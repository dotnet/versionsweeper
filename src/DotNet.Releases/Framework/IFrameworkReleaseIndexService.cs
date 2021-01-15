using System.Collections.Generic;

namespace DotNet.Versions
{
    public interface IFrameworkReleaseIndexService
    {
        HashSet<string> FrameworkReseaseFileNames { get; }
    }
}
