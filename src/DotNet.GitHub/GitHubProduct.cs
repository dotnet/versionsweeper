// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Octokit;

namespace DotNet.GitHub
{
    public static class GitHubProduct
    {
        static readonly string _name = "DotNetVersionSweeper";
        static readonly string _version = "1.0";

        public static ProductHeaderValue Header { get; } = new(_name, _version);
    }
}
