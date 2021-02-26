// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
                .AddSingleton<IResilientGitHubClientFactory, DefaultResilientGitHubClientFactory>()
                .AddSingleton<GitHubGraphQLClient>()
                .AddSingleton<RateLimitAwareQueue>()
                .AddSingleton<IGitHubIssueService, GitHubIssueService>()
                .AddSingleton<IGitHubLabelService, GitHubLabelService>();
        }
    }
}
