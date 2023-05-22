// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Models;

public sealed class Solution
{
    public string FullPath { get; set; } = null!;

    public HashSet<Project> Projects { get; } = new();
}
