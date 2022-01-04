// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public class RateLimitAwareQueue
{
    const int DelayBetweenPostCalls = 1_000;

    readonly HashSet<string> _unqiueNewIssueTitles = new(StringComparer.OrdinalIgnoreCase);
    readonly ConcurrentQueue<(GitHubApiArgs, NewIssue)> _newIssuesQueue = new();
    readonly ConcurrentQueue<(GitHubApiArgs, IssueUpdate)> _updateIssuesQueue = new();
    readonly IGitHubIssueService _gitHubIssueService;

    public RateLimitAwareQueue(IGitHubIssueService gitHubIssueService) =>
        _gitHubIssueService = gitHubIssueService;

    public void Enqueue(GitHubApiArgs args, NewIssue issue)
    {
        if (_unqiueNewIssueTitles.Add(issue.Title))
        {
            _newIssuesQueue.Enqueue((args, issue));
        }
    }

    public void Enqueue(GitHubApiArgs args, IssueUpdate issue) =>
        _updateIssuesQueue.Enqueue((args, issue));

    public async IAsyncEnumerable<(string Type, Issue Issue)> ExecuteAllQueuedItemsAsync()
    {
        while (_newIssuesQueue is { IsEmpty: false }
            && _newIssuesQueue.TryDequeue(out (GitHubApiArgs, NewIssue) newItem))
        {
            var (args, newIssue) = newItem;
            var issue = await _gitHubIssueService.PostIssueAsync(
                args.Owner, args.RepoName, args.Token, newIssue);

            yield return ("Created", issue);

            await Task.Delay(DelayBetweenPostCalls);
        }

        while (_updateIssuesQueue is { IsEmpty: false }
            && _updateIssuesQueue.TryDequeue(out (GitHubApiArgs, IssueUpdate) updatedItem))
        {
            var (args, newIssue) = updatedItem;
            var issue = await _gitHubIssueService.UpdateIssueAsync(
                args.Owner, args.RepoName, args.Token, args.IssueNumber, newIssue);

            yield return ("Updated", issue);

            await Task.Delay(DelayBetweenPostCalls);
        }
    }
}
