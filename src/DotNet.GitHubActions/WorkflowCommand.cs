using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNet.GitHubActions
{
    public record WorkflowCommand(
        string Command,
        object? Message,
        IDictionary<string, string>? CommandProperties = default)
    {
        const string CMD_STRING = "::";

        public override string ToString()
        {
            StringBuilder builder = new($"{CMD_STRING}{Command}");

            if (CommandProperties?.Any() ?? false)
            {
                foreach (var (first, key, value) 
                    in CommandProperties.Select((kvp, i) => (i == 0, kvp.Key, kvp.Value)))
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }
                    builder.Append($"{key}={EscapeProperty(value)}");
                }
            }

            builder.Append($"{CMD_STRING}{EscapeData(Message)}");

            return builder.ToString();
        }

        static string EscapeData(object? value) =>
            value.ToCommandValue()
                .Replace("%", "%25")
                .Replace("\r", "%0D")
                .Replace("\n", "%0A");

        static string EscapeProperty(object? value) =>
            value.ToCommandValue()
                .Replace("%", "%25")
                .Replace("\r", "%0D")
                .Replace("\n", "%0A")
                .Replace(":", "%3A")
                .Replace(",", "%2C");
    }

    static class GenericExtensions
    {
        static readonly Lazy<JsonSerializerOptions> _lazyOptions = new(() => new()
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        });

        internal static string ToCommandValue<T>(this T? value) => value switch
        {
            string str => str,
            _ => JsonSerializer.Serialize(value, _lazyOptions.Value)
        };
    }
}