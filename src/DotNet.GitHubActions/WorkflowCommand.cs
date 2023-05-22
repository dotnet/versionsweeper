// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNet.GitHubActions;

/// <summary>
/// {commandName} is the command name, example:
///     "::set-output::"
///     "::{commandName}::"
///      when {commandName} is "set-output"
/// {message} is a string, example:
///     "::set-output::Hello World!"
///     "::set-output::{message}"
///     when {message} is "Hello World!"
/// {properties} is a Dictionary{string, string}, example:
///     "::set-output prop1=Value 1, value 2::Hi"
///     "::set-output {properties}::{message}"
///     when {message} is "Hi"
///     and {properties} is
///         new Dictionary{string, string}
///         {
///             ["prop1] = "Value 1",
///             ["prop2"] = "Value 2"
///         }
/// ::{commandName} {properties}::{message}
/// </summary>
public record WorkflowCommand<T>(
    string CommandName,
    T? Message,
    IDictionary<string, string>? CommandProperties = default)
{
    const string CMD_STRING = "::";

    public override string ToString()
    {
        StringBuilder builder = new($"{CMD_STRING}{CommandName}");

        if (CommandProperties?.Any() ?? false)
        {
            foreach ((bool first, string key, string value)
                in CommandProperties.Select((kvp, i) => (i == 0, kvp.Key, kvp.Value)))
            {
                if (!first)
                {
                    builder.Append(',');
                }
                builder.Append($" {key}={EscapeProperty(value)}");
            }
        }

        builder.Append($"{CMD_STRING}{EscapeData(Message)}");

        return builder.ToString();
    }

    static string EscapeData<TSource>(TSource? value) =>
        value.ToCommandValue()
            .Replace("%", "%25")
            .Replace("\r", "%0D")
            .Replace("\n", "%0A");

    static string EscapeProperty<TSource>(TSource? value) =>
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
        null => string.Empty,
        string str => str,
        _ => JsonSerializer.Serialize(value, _lazyOptions.Value)
    };
}
