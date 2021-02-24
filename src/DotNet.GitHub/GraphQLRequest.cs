using DotNet.Extensions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
