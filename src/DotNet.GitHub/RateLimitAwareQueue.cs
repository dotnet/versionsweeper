// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace DotNet.GitHub
{
    public class RateLimitAwareQueue
    {
        const int DelayBetweenPostCalls = 1_000;

        readonly ConcurrentQueue<(GitHubApiArgs, NewIssue)> _issuesQueue = new();
        readonly IGitHubIssueService _gitHubIssueService;

        public RateLimitAwareQueue(IGitHubIssueService gitHubIssueService) =>
            _gitHubIssueService = gitHubIssueService;

        public void Enqueue(GitHubApiArgs args, NewIssue issue) =>
            _issuesQueue.Enqueue((args, issue));

        public async IAsyncEnumerable<Issue> ExecuteAllQueuedItemsAsync()
        {
            while (_issuesQueue is { IsEmpty: false }
                && _issuesQueue.TryDequeue(out var item))
            {
                var (args, newIssue) = item;
                var issue = await _gitHubIssueService.PostIssueAsync(
                    args.Owner, args.RepoName, args.Token, newIssue);

                yield return issue;

                await Task.Delay(DelayBetweenPostCalls);
            }
        }
    }
}
