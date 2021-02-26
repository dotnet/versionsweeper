﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using DotNet.Extensions;

namespace DotNet.GitHub
{
    public record GraphQLRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; init; } = "";

        [JsonPropertyName("variables")]
        public Dictionary<string, string> Variables { get; init; } = new();

        public override string ToString() => this.ToJson()!;
    }
}
