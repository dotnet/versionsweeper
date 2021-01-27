using DotNet.Extensions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DotNet.GitHub
{
    public class GraphQLRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; set; } = "";

        [JsonPropertyName("variables")]
        public IDictionary<string, string> Variables { get; } = new Dictionary<string, string>();

        public override string ToString() => this.ToJson()!;
    }
}
