using Octokit;
using Octokit.Extensions;
using System.Threading.Tasks;

namespace DotNet.GitHub
{
    public class GitHubIssueService : IGitHubIssueService
    {
        readonly ResilientGitHubClientFactory _clientFactory;

        public GitHubIssueService(
            ResilientGitHubClientFactory clientFactory) =>
            _clientFactory = clientFactory;

        public async ValueTask<Issue> PostIssueAsync(
            string owner, string name, string token, NewIssue newIssue)
        {
            var client = _clientFactory.Create(
                productHeaderValue: new("dotnet-versionsweeper"),
                credentials: new(token));

            var issue = await client.Issue.Create(owner, name, newIssue);
            return issue;
        }
    }
}
