// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public static class GitHubProduct
{
    const string Name = "DotNetVersionSweeper";
    const string Version = "1.2";

    public static ProductHeaderValue Header { get; } = new(Name, Version);
}
