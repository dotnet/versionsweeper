using Octokit;

namespace DotNet.GitHub
{
    public interface IResilientGitHubClientFactory
    {
        IGitHubClient Create(string token);
    }
}
