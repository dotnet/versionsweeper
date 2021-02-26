// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DotNet.IO
{
    internal class ProjectJson
    {
        [JsonPropertyName("frameworks")]
        public Dictionary<string, Framework> Frameworks { get; set; } = new();
    }

    internal class Framework
    {
    }
}
