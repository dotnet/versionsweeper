using System.Collections.Generic;

namespace DotNet.Versions
{
    public record ProjectSupportReport(
        string ProjectPath,
        HashSet<TargetFrameworkMonikerSupport> TargetFrameworkMonikerSupports);

    public record TargetFrameworkMonikerSupport(
        string TargetFrameworkMoniker,
        string Version,
        bool IsUnsupported);
}
