// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.VersionSweeper;

public enum ActionType
{
    /// <summary>
    /// Only create issues, never pull requests. Issues are formatted appropriately
    /// with markdown, tables, links, etc.
    /// </summary>
    CreateIssue,

    /// <summary>
    /// Only create pull requests, never issues. Pull requests rely on the
    /// upgrade-assistant to make the appropriate changes.
    /// </summary>
    PullRequest,

    /// <summary>
    /// Create both issues and pull requests whenever possible.
    /// </summary>
    All
}
