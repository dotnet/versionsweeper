// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Text.Json.JsonSerializer;

namespace DotNet.Extensions
{
    public static class ObjectExtensions
    {
        static readonly Lazy<JsonSerializerOptions> _lazyOptions = new(() => new()
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
        });

        public static string? ToJson(this object value, JsonSerializerOptions? options = default) =>
            value is null ? null : Serialize(value, options ?? _lazyOptions.Value);

        public static T? FromJson<T>(this string? json, JsonSerializerOptions? options = default) =>
            string.IsNullOrWhiteSpace(json) ? default : Deserialize<T>(json, options ?? _lazyOptions.Value);

        public static DateTime? ToDateTime(this string? value) =>
            value is null ? default : DateTime.TryParse(value, out var dateTime) ? dateTime : null;

        public static void Deconstruct<T>(
            this T? nullable, out bool hasValue, out T value) where T : struct =>
            (hasValue, value) = (nullable.HasValue, nullable.GetValueOrDefault());
    }
}
