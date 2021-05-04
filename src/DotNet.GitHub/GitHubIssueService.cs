// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit;

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

            _logger.LogInformation($"Issue created: {issue.HtmlUrl}");

            return issue;
        }

        public async ValueTask<Issue> UpdateIssueAsync(
            string owner, string name, string token, long number, IssueUpdate issueUpdate)
        {
            var issuesClient = GetIssuesClient(token);

            // The GitHub GraphQL API returns a long for the issue Id.
            // The GitHub REST API expects an int for the issue Id.

            var issue = await issuesClient.Update(
                owner, name, unchecked((int)number), issueUpdate);

            _logger.LogInformation($"Issue updated: {issue.HtmlUrl}");

            return issue;
        }

        IIssuesClient GetIssuesClient(string token)
        {
            var client = _clientFactory.Create(token);
            return client.Issue;
        }
    }
}
