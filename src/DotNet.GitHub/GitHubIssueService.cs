using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Extensions;
using System.Threading.Tasks;

namespace DotNet.GitHub
{
    public class GitHubIssueService : IGitHubIssueService
    {
        readonly ResilientGitHubClientFactory _clientFactory;
        readonly ILogger<GitHubIssueService> _logger;

        public GitHubIssueService(
            ResilientGitHubClientFactory clientFactory,
            ILogger<GitHubIssueService> logger) =>
            (_clientFactory, _logger) = (clientFactory, logger);

        public async ValueTask<Issue> PostIssueAsync(
            string owner, string name, string token, NewIssue newIssue)
        {
            var client = _clientFactory.Create(
                productHeaderValue: Product.Header,
                credentials: new(token));

            var issue = await client.Issue.Create(owner, name, newIssue);

            _logger.LogInformation(
                $"Issue created: {issue.HtmlUrl}");

            return issue;
        }
    }
}
