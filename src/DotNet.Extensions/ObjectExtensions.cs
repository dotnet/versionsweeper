// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Extensions;

public static class ObjectExtensions
{
    public static string? ToJson<T>(this T value, JsonTypeInfo<T> jsonTypeInfo) =>
        value is null ? null : Serialize(value, jsonTypeInfo);

    public static T? FromJson<T>(this string? json, JsonTypeInfo<T> jsonTypeInfo) =>
        string.IsNullOrWhiteSpace(json) ? default : Deserialize<T>(json, jsonTypeInfo);

    public static DateTime? ToDateTime(this string? value) =>
        value is null ? default : DateTime.TryParse(value, out global::System.DateTime dateTime) ? dateTime : null;

    public static void Deconstruct<T>(
        this T? nullable, out bool hasValue, out T value) where T : struct =>
        (hasValue, value) = (nullable.HasValue, nullable.GetValueOrDefault());
}
