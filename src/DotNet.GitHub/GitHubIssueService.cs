using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Extensions;
using System.Threading.Tasks;

namespace DotNet.GitHub
{
    public class GitHubIssueService : IGitHubIssueService
    {
        readonly IResilientGitHubClientFactory _clientFactory;
        readonly IGitHubLabelService _gitHubLabelService;
        readonly ILogger<GitHubIssueService> _logger;

        public GitHubIssueService(
            IResilientGitHubClientFactory clientFactory,
            IGitHubLabelService gitHubLabelService,
            ILogger<GitHubIssueService> logger) =>
            (_clientFactory, _gitHubLabelService, _logger) = (clientFactory, gitHubLabelService, logger);

        public async ValueTask<Issue> PostIssueAsync(
            string owner, string name, string token, NewIssue newIssue)
        {
            var issuesClient = GetIssuesClient(token);

            var label = await _gitHubLabelService.GetOrCreateLabelAsync(owner, name, token);
            newIssue.Labels.Add(label.Name);

            var issue = await issuesClient.Create(owner, name, newIssue);

            _logger.LogInformation(
                $"Issue created: {issue.HtmlUrl}");

            return issue;
        }

        IIssuesClient GetIssuesClient(string token)
        {
            var client = _clientFactory.Create(token);
            return client.Issue;
        }
    }
}
