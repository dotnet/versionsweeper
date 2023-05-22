// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

class DefaultLabel
{
    internal const string Name = "dotnet-target-version";
    internal const string Color = "512bd4";
    internal const string Description =
        "Issues and PRs automatically generated from the .NET version sweeper.";

    internal static NewLabel Value { get; } = new(Name, Color)
    {
        Description = Description
    };
}
