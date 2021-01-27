using Microsoft.Extensions.DependencyInjection;
using Octokit.Extensions;

namespace DotNet.GitHub
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotNetGitHubServices(
            this IServiceCollection services)
        {
            services.AddHttpClient<GitHubGraphQLClient>();

            return services
                .AddSingleton<ResilientGitHubClientFactory>()
                .AddSingleton<GitHubGraphQLClient>()
                .AddSingleton<IGitHubIssueService, GitHubIssueService>();
        }
    }
}
