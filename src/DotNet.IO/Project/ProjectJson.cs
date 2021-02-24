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
