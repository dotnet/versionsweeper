// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public record GraphQLResult<T>
{
    public Data<T>? Data { get; init; }
}

public record Data<T>
{
    public Search<T>? Search { get; init; }
}

public record Search<T>
{
    public T[]? Nodes { get; init; }
}
