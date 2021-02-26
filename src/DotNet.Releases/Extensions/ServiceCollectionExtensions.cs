// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.Releases;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotNetReleaseServices(
            this IServiceCollection services) =>
            services
                .AddMemoryCache()
                .AddSingleton<ICoreReleaseIndexService, CoreReleaseIndexService>()
                .AddSingleton<IFrameworkReleaseIndexService, FrameworkReleaseIndexService>()
                .AddSingleton<IFrameworkReleaseService, FrameworkReleaseService>()
                .AddSingleton<IUnsupportedProjectReporter, UnsupportedProjectReporter>();
    }
}
