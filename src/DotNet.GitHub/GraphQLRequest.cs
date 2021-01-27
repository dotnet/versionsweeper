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
        public IDictionary<string, string> Variables { get; init; } = new Dictionary<string, string>();

        public override string ToString() => this.ToJson()!;
    }
}
