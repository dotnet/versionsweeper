// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using DotNet.Extensions;
using DotNet.GitHub;
using Octokit;
using Xunit;

namespace DotNet.GitHubTests;

public sealed class GraphQLRequestTests
{
    static readonly JsonSerializerOptions s_options = new()
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void RequestObjectCorrectlySerializesToJsonTest()
    {
        GraphQLRequest request = new()
        {
            Query = @"query {
  viewer {
    login
  }
}"
        };

        string expectedJson = @"{""query"":""query {
  viewer {
    login
  }
}"",""variables"":{}}";
        string actualJson = Regex.Unescape(request.ToString());

        Assert.Equal(expectedJson, actualJson);
    }

    [Fact]
    public void ResponseJsonCorrectlyDeserializesTest()
    {
        const string responseJson = @"{
    ""data"": {
        ""search"": {
            ""nodes"": [
                {
                    ""title"": ""Update generation.csproj from .NET Core 2.2 to LTS(or current) version"",
                    ""number"":4141,
                    ""url"":""https://github.com/dotnet/samples/issues/4141"",
                    ""state"":""OPEN"",
                    ""createdAt"":""2021-01-25T20:49:23Z"",
                    ""updatedAt"":""2021-01-25T20:49:23Z"",
                    ""closedAt"":null
                }
            ]
        }
    }
}";
        ExistingIssue expectedIssue = new()
        {
            Title = "Update generation.csproj from .NET Core 2.2 to LTS(or current) version",
            Number = 4141,
            Url = "https://github.com/dotnet/samples/issues/4141",
            State = ItemState.Open,
            CreatedAt = new DateTime(2021, 1, 25, 20, 49, 23, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2021, 1, 25, 20, 49, 23, DateTimeKind.Utc),
            ClosedAt = null
        };

        ExistingIssue actualIssue =
            responseJson.FromJson<GraphQLResult<ExistingIssue>>(s_options)
                .Data
                .Search
                .Nodes[0];

        Assert.Equal(expectedIssue, actualIssue);
    }
}
