// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.IO;

internal sealed class ProjectJson
{
    [JsonPropertyName("frameworks")]
    public Dictionary<string, Framework> Frameworks { get; set; } = new();
}

internal sealed class Framework
{
}
