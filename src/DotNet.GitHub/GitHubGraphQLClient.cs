using DotNet.Extensions;
using Octokit;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        public GitHubGraphQLClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<ExistingIssue?> GetIssueAsync(
            string owner, string name, string token, string title)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new("Token", token);
            _httpClient.DefaultRequestHeaders.UserAgent.Add(
                new(Product.Header.Name, Product.Header.Version));

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

            var json = await response.Content.ReadAsStringAsync();
            var result = json.FromJson<GraphQLResult<ExistingIssue>>(_options);

            return result?.Data?.Search?.Nodes
                ?.Where(i => i.State == ItemState.Open)
                ?.OrderByDescending(i => i.CreatedAt.GetValueOrDefault())
                ?.FirstOrDefault();
        }
    }
}
