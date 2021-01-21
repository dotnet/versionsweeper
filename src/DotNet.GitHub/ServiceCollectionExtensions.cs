using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace DotNet.GitHub
{
    public static class ServiceCollectionExtensions
    {
        static readonly ProductHeaderValue _header = new("dotnet-versionsweeper");

        public static IServiceCollection AddDotNetGitHubServices(
            this IServiceCollection services) =>
            services.AddSingleton<GitHubClient>(provider => new(_header))
                .AddSingleton<IGitHubIssueService, GitHubIssueService>();
    }
}
