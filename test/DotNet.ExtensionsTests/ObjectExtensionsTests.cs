// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using Xunit;

namespace DotNet.Extensions.Tests;

public sealed class ObjectExtensionsTests
{
    public static IEnumerable<object[]> FromJsonInput =
    [
            [
                "{ \"test.value\": \"I'm from Wisconsin.\" }",
                new CustomName { TestValue = "I'm from Wisconsin." },
                new Func<CustomName, CustomName, bool>((name1, name2) => name1.TestValue == name2.TestValue)
            ],
        ];

    [
        Theory,
        MemberData(nameof(FromJsonInput))
    ]
    public void FromJsonTest(string json, CustomName expected, Func<CustomName, CustomName, bool> customEqual)
    {
        Assert.True(customEqual.Invoke(expected, json.FromJson(TestJsonSerializerContext.Default.CustomName)));
    }

    public static IEnumerable<object[]> ToJsonInput =
    [
            [
                new EnumWithValue(TestEnum.ButThisIsAlsoOne),
                "{\"value\":\"one\"}",
            ],
            [
                new CustomName { TestValue = "I am from Wisconsin." },
                "{\"test.value\":\"I am from Wisconsin.\"}"
            ],
        ];

    [
        Theory,
        MemberData(nameof(ToJsonInput))
    ]
    public void ToJsonTest(object value, string expected) =>
        Assert.Equal(expected, value.ToJson(TestJsonSerializerContext.Default.Object), true);

    public static IEnumerable<object[]> ToDateTimeInput =
    [
            ["2021-01-12", new DateTime(2021, 1, 12)],
            ["2022-12-03", new DateTime(2022, 12, 3)],
            ["2019-02-12", new DateTime(2019, 2, 12)],
            [default(string), new DateTime?()],
            ["...", null],
        ];

    [
        Theory,
        MemberData(nameof(ToDateTimeInput))
    ]
    public void ToDateTime(string value, DateTime? expected) =>
        Assert.Equal(expected, value.ToDateTime());
}

enum TestEnum
{
    Zero = 0,

    One = 1,
    ButThisIsAlsoOne = 1,

    Two = 2,
    WaitWhat = 2
}

record class EnumWithValue(TestEnum Value);

public sealed class CustomName
{
    [JsonPropertyName("test.value")] public string TestValue { get; init; }
}

[JsonSourceGenerationOptions(
    Converters = [typeof(JsonStringEnumConverter)])]
[JsonSerializable(typeof(CustomName))]
[JsonSerializable(typeof(object[]))]
[JsonSerializable(typeof(TestEnum))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(EnumWithValue))]
internal partial class TestJsonSerializerContext : JsonSerializerContext;
