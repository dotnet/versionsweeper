using Octokit;
using System.Threading.Tasks;

namespace DotNet.GitHub
{
    public class GitHubIssueService : IGitHubIssueService
    {
        readonly GitHubClient _client;

        public GitHubIssueService(GitHubClient client) => _client = client;

        async ValueTask<Issue> IGitHubIssueService.PostIssueAsync(
            string owner, string name, string token, NewIssue issue)
        {
            _client.Credentials = new(token);
            var resultingIssue = await _client.Issue.Create(owner, name, issue);
            return resultingIssue;
        }
    }
}
