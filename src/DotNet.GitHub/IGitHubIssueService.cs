using Octokit;
using System.Threading.Tasks;

namespace DotNet.GitHub
{
    public interface IGitHubIssueService
    {
        ValueTask<Issue> PostIssueAsync(
            string owner, string name, string token, NewIssue issue);
    }
}
