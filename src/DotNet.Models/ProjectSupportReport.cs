// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace DotNet.Models
{
    public record ProjectSupportReport(
        Project Project,
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
