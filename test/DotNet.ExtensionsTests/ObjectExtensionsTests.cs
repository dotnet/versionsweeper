// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using Xunit;

namespace DotNet.Extensions.Tests;

public sealed class ObjectExtensionsTests
{
    public static IEnumerable<object[]> FromJsonInput = new[]
    {
            new object[]
            {
                "{ \"value\": \"one\" }", new { Value = TestEnum.ButThisIsAlsoOne }
            },
            [
                "{ \"test.value\": \"I'm from Wisconsin.\" }",
                new CustomName { TestValue = "I'm from Wisconsin." },
                new Func<CustomName, CustomName, bool>((name1, name2) => name1.TestValue == name2.TestValue)
            ],
        };

    [
        Theory,
        MemberData(nameof(FromJsonInput))
    ]
    public void FromJsonTest<T>(string json, T expected, Func<T, T, bool> customEqual = default)
    {
        if (customEqual is null)
        {
            Assert.Equal(expected, json.FromJson<T>());
        }
        else
        {
            Assert.True(customEqual.Invoke(expected, json.FromJson<T>()));
        }
    }

    public static IEnumerable<object[]> ToJsonInput = new[]
    {
            new object[]
            {
                new { Value = TestEnum.ButThisIsAlsoOne },
                "{\"value\":\"one\"}",
            },
            [
                new CustomName { TestValue = "I'm from Wisconsin." },
                "{\"test.value\":\"I'm from Wisconsin.\"}"
            ],
        };

    [
        Theory,
        MemberData(nameof(ToJsonInput))
    ]
    public void ToJsonTest<T>(T value, string expected) =>
        Assert.Equal(expected, value.ToJson(), true);

    public static IEnumerable<object[]> ToDateTimeInput = new[]
    {
            new object[] { "2021-01-12", new DateTime(2021, 1, 12) },
            ["2022-12-03", new DateTime(2022, 12, 3)],
            ["2019-02-12", new DateTime(2019, 2, 12)],
            [default(string), new DateTime?()],
            ["...", null],
        };

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

public sealed class CustomName
{
    [JsonPropertyName("test.value")] public string TestValue { get; init; }
}
