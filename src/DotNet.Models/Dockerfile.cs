// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Models;

public record Dockerfile
{
    /// <summary>
    /// The fully qualified path of the <i>Dockerfile</i>.
    /// </summary>
    public string FullPath { get; init; } = default!;

    /// <summary>
    /// The image details for each <c>FROM</c> instruction in the <i>Dockerfile</i>.
    /// </summary>
    public ISet<ImageDetails>? ImageDetails { get; init; }
}
