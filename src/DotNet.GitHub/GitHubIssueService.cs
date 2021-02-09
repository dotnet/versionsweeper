using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Extensions;
using System.Threading.Tasks;

namespace DotNet.GitHub
{
    public class GitHubIssueService : IGitHubIssueService
    {
        readonly ResilientGitHubClientFactory _clientFactory;
        readonly GitHubLabelService _gitHubLabelService;
        readonly ILogger<GitHubIssueService> _logger;

        public GitHubIssueService(
            ResilientGitHubClientFactory clientFactory,
            GitHubLabelService gitHubLabelService,
            ILogger<GitHubIssueService> logger) =>
            (_clientFactory, _gitHubLabelService, _logger) = (clientFactory, gitHubLabelService, logger);

        public async ValueTask<Issue> PostIssueAsync(
            string owner, string name, string token, NewIssue newIssue)
        {
            var issuesClient = GetIssuesClient(token);
            var issue = await issuesClient.Create(owner, name, newIssue);
            var label = await _gitHubLabelService.GetOrCreateLabelAsync(owner, name, token);
            _ = await issuesClient.Labels.AddToIssue(owner, name, issue.Number, new[] { label.Name });

            _logger.LogInformation(
                $"Issue created: {issue.HtmlUrl}");

            return issue;
        }

        IIssuesClient GetIssuesClient(string token)
        {
            var client = _clientFactory.Create(
                productHeaderValue: Product.Header,
                credentials: new(token));

            return client.Issue;
        }
    }
}
