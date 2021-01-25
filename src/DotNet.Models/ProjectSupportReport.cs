using System.Collections.Generic;

namespace DotNet.Models
{
    public record ProjectSupportReport(
        string ProjectPath,
        HashSet<TargetFrameworkMonikerSupport> TargetFrameworkMonikerSupports);

    public record TargetFrameworkMonikerSupport(
        string TargetFrameworkMoniker,
        string Version,
        bool IsUnsupported,
        IRelease Release)
    {
        public string NearestLtsVersion { get; set; } = null!;
    }
}
