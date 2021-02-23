using Octokit;
using Octokit.Extensions;

namespace DotNet.GitHub
{
    public class DefaultResilientGitHubClientFactory : IResilientGitHubClientFactory
    {
        readonly ResilientGitHubClientFactory _clientFactory;

        public DefaultResilientGitHubClientFactory(
            ResilientGitHubClientFactory clientFactory) =>
            _clientFactory = clientFactory;

        public IGitHubClient Create(string token) =>
            _clientFactory.Create(
                productHeaderValue: GitHubProduct.Header,
                credentials: new(token));
    }
}
