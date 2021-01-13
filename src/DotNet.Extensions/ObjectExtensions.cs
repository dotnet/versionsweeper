using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Text.Json.JsonSerializer;

namespace DotNet.Extensions
{
    public static class ObjectExtensions
    {
        static readonly HyphenatedJsonNamingPolicy _jsonNamingPolicy = new();
        static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = _jsonNamingPolicy,
            NumberHandling =
                    JsonNumberHandling.AllowReadingFromString |
                    JsonNumberHandling.WriteAsString,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        public static string? ToJson(this object value) =>
            value is null ? null : Serialize(value, _options);

        public static T? FromJson<T>(this string? json) =>
            string.IsNullOrWhiteSpace(json) ? default : Deserialize<T>(json, _options);
    }
}
