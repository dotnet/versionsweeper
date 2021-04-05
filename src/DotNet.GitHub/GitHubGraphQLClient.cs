// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotNet.Extensions;
using Microsoft.Extensions.Logging;
using Octokit;

namespace DotNet.GitHub
{
    public class GitHubGraphQLClient
    {
        const string _issueQuery = @"query($search_value: String!) {
  search(type: ISSUE, query: $search_value, first: 10) {
    nodes {
      ... on Issue {
        title
        number
        url
        state
        createdAt
        updatedAt
        closedAt
      }
    }
  }
}";

        readonly Uri _graphQLUri = new("https://api.github.com/graphql");
        readonly static JsonSerializerOptions _options = new()
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            PropertyNameCaseInsensitive = true
        };

        readonly HttpClient _httpClient;
        readonly ILogger<GitHubGraphQLClient> _logger;

        public GitHubGraphQLClient(HttpClient httpClient, ILogger<GitHubGraphQLClient> logger) =>
            (_httpClient, _logger) = (httpClient, logger);

        public async Task<ExistingIssue?> GetIssueAsync(
            string owner, string name, string token, string title)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Token", token);
                _httpClient.DefaultRequestHeaders.UserAgent.Add(
                    new(GitHubProduct.Header.Name, GitHubProduct.Header.Version));

                GraphQLRequest graphQLRequest = new()
                {
                    Query = _issueQuery,
                    Variables =
                    {
                        ["search_value"] = $"repo:{owner}/{name} type:issue '{title}' in:title"
                    }
                };

                using var request = new StringContent(graphQLRequest.ToString());
                request.Headers.ContentType = new(MediaTypeNames.Application.Json);
                request.Headers.Add("Accepts", MediaTypeNames.Application.Json);

                using var response = await _httpClient.PostAsync(_graphQLUri, request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = json.FromJson<GraphQLResult<ExistingIssue>>(_options);

                return result?.Data?.Search?.Nodes
                    ?.Where(i => i.State == ItemState.Open)
                    ?.OrderByDescending(i => i.CreatedAt.GetValueOrDefault())
                    ?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message);

                return default;
            }
        }
    }
}
