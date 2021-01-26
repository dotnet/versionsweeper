using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Octokit.Extensions;

namespace DotNet.GitHub
{
    public static class ServiceCollectionExtensions
    {
        static readonly ProductHeaderValue _header = new ProductHeaderValue("dotnet-versionsweeper");

        public static IServiceCollection AddDotNetGitHubServices(
            this IServiceCollection services) =>
            services.AddSingleton<ResilientGitHubClientFactory>()
                .AddSingleton<IGitHubIssueService, GitHubIssueService>();
    }
}
