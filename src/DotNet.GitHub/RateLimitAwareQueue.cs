using Nito.AsyncEx;
using Octokit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet.GitHub
{
    public class RateLimitAwareQueue
    {
        const int DelayBetweenPostCalls = 1_000;

        readonly AsyncProducerConsumerQueue<(GitHubApiArgs, NewIssue)> _issuesQueue = new();
        readonly IGitHubIssueService _gitHubIssueService;

        public RateLimitAwareQueue(IGitHubIssueService gitHubIssueService) =>
            _gitHubIssueService = gitHubIssueService;


        public Task EnqueueAsync(GitHubApiArgs args, NewIssue issue) =>
            _issuesQueue.EnqueueAsync((args, issue));

        public async IAsyncEnumerable<Issue> ExecuteAllQueuedItemsAsync()
        {
            _issuesQueue.CompleteAdding();

            var hasItems = await _issuesQueue.OutputAvailableAsync();
            while (hasItems)
            {
                var (args, newIssue) = await _issuesQueue.DequeueAsync();
                var issue = await _gitHubIssueService.PostIssueAsync(
                    args.Owner, args.Name, args.Token, newIssue);

                yield return issue;

                await Task.Delay(DelayBetweenPostCalls);
                hasItems = await _issuesQueue.OutputAvailableAsync();
            }
        }
    }
}
