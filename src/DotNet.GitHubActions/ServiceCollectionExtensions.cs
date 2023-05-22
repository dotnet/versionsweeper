// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace DotNet.GitHubActions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGitHubActionServices(
        this IServiceCollection services) =>
        services.AddSingleton<IJobService, JobService>();
}
