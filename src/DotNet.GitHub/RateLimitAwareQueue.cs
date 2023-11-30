// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public sealed class RateLimitAwareQueue(IGitHubIssueService gitHubIssueService)
{
    const int DelayBetweenPostCalls = 1_000;

    readonly HashSet<string> _uniqueTitles = new(StringComparer.OrdinalIgnoreCase);
    readonly ConcurrentQueue<(GitHubApiArgs, NewIssue)> _newIssuesQueue = new();
    readonly ConcurrentQueue<(GitHubApiArgs, IssueUpdate)> _updateIssuesQueue = new();

    public void Enqueue(GitHubApiArgs args, NewIssue issue)
    {
        if (_uniqueTitles.Add(issue.Title))
        {
            _newIssuesQueue.Enqueue((args, issue));
        }
    }

    public void Enqueue(GitHubApiArgs args, IssueUpdate issue) =>
        _updateIssuesQueue.Enqueue((args, issue));

    public async IAsyncEnumerable<(string Message, string Url)> ExecuteAllQueuedItemsAsync()
    {
        while (_newIssuesQueue is { IsEmpty: false }
            && _newIssuesQueue.TryDequeue(out (GitHubApiArgs, NewIssue) newItem))
        {
            var (args, newIssue) = newItem;
            var issue = await gitHubIssueService.PostIssueAsync(
                args.Owner, args.RepoName, args.Token, newIssue);

            yield return ("Created issue", issue.HtmlUrl);

            await Task.Delay(DelayBetweenPostCalls);
        }

        while (_updateIssuesQueue is { IsEmpty: false }
            && _updateIssuesQueue.TryDequeue(out (GitHubApiArgs, IssueUpdate) updatedItem))
        {
            var (args, newIssue) = updatedItem;
            var issue = await gitHubIssueService.UpdateIssueAsync(
                args.Owner, args.RepoName, args.Token, args.IssueNumber, newIssue);

            yield return ("Updated issue", issue.HtmlUrl);

            await Task.Delay(DelayBetweenPostCalls);
        }
    }
}
