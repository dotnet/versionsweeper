// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public record ExistingIssue
{
    public string? Title { get; init; }
    public long Number { get; init; }
    public string? Url { get; init; }
    public ItemState State { get; init; }
    public string? Body { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? ClosedAt { get; init; }
}
