// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHubActions;

/// <summary>
/// Inspired by <a href="https://github.com/actions/toolkit/blob/main/packages/core/src/core.ts"></a>
/// </summary>
public readonly record struct InputOptions
{
    public InputOptions()
    {
        Required = false;
        TrimWhitespace = true;
    }

    /// <summary>
    /// Optional. Whether the input is required. If required and not present, will throw. Defaults to false.
    /// </summary>
    public bool? Required { get; init; }

    /// <summary>
    /// Optional. Whether leading/trailing whitespace will be trimmed for the input. Defaults to true 
    /// </summary>
    public bool? TrimWhitespace { get; init; }
}
