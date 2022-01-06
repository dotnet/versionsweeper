// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHubActions;

public record InputOptions
{
    /// <summary>
    /// Optional. Whether the input is required. If required and not present, will throw. Defaults to false
    /// </summary>
    public bool? Required { get; init; }
}
