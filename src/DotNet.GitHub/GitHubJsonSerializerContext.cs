﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    Converters = [
        typeof(JsonStringEnumConverter<ItemState>),
    ]
)]
[JsonSerializable(typeof(GraphQLRequest))]
[JsonSerializable(typeof(ExistingIssue))]
[JsonSerializable(typeof(GraphQLResult<ExistingIssue>))]
internal partial class GitHubJsonSerializerContext : JsonSerializerContext;
