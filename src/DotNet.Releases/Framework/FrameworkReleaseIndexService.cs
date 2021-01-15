using System.Collections.Generic;

namespace DotNet.Versions
{
    internal class FrameworkReleaseIndexService : IFrameworkReleaseIndexService
    {
        /// <summary>
        /// Private sourhttps://github.com/dotnet/website-resources/tree/master/data/dotnet-framework-releases
        /// </summary>
        public HashSet<string> FrameworkReseaseFileNames { get; } = new()
        {
            "net35-sp1.json",
            "net40.json",
            "net45.json",
            "net451.json",
            "net452.json",
            "net46.json",
            "net461.json",
            "net462.json",
            "net47.json",
            "net471.json",
            "net472.json",
            "net48.json"
        };
    }
}
