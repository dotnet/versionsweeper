﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.VersionSweeper
{
    public static class EnvironmentVariableNames
    {
        public static class GitHub
        {
            public const string Token = "GITHUB_TOKEN";
        }

        public static class Sweeper
        {
            public const string Owner = "OWNER";
            public const string Name = "NAME";
            public const string Branch = "BRANCH";
            public const string Directory = "DIR";
            public const string SearchPattern = "PATTERN";
        }
    }
}
