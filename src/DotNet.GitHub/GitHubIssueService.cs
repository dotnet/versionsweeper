// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public sealed class GitHubIssueService : IGitHubIssueService
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
        Label label = await _gitHubLabelService.GetOrCreateLabelAsync(owner, name, token);
        newIssue.Labels.Add(label.Name);

        IIssuesClient issuesClient = GetIssuesClient(token);

        try
        {
            Issue issue = await issuesClient.Create(owner, name, newIssue);

            _logger.LogInformation("Issue created: {HtmlUrl}", issue.HtmlUrl);

            return issue;
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Failed to create issue: {Owner}/{Name} {Title}",
                owner, name, newIssue.Title);

            throw;
        }
    }

    public async ValueTask<Issue> UpdateIssueAsync(
        string owner, string name, string token, long number, IssueUpdate issueUpdate)
    {
        IIssuesClient issuesClient = GetIssuesClient(token);

        // The GitHub GraphQL API returns a long for the issue Id.
        // The GitHub REST API expects an int for the issue Id.

        try
        {
            Issue issue = await issuesClient.Update(
                owner, name, unchecked((int)number), issueUpdate);

            _logger.LogInformation("Issue updated: {HtmlUrl}", issue.HtmlUrl);

            return issue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update issue: {Owner}/{Name}#{Number}",
                owner, name, number);

            throw;
        }
    }

    IIssuesClient GetIssuesClient(string token) => _clientFactory.Create(token).Issue;
}
