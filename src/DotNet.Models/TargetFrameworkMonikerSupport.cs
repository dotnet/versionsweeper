// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Models;

public record TargetFrameworkMonikerSupport(
    string TargetFrameworkMoniker,
    string Version,
    bool IsUnsupported,
    IRelease Release)
{
    public string NearestLtsVersion { get; set; } = null!;
}
