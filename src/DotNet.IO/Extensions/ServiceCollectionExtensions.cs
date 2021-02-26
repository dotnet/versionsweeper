// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.IO;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotNetFileSystem(
            this IServiceCollection services) =>
            services
                .AddSingleton<IProjectFileReader, ProjectFileReader>()
                .AddSingleton<ISolutionFileReader, SolutionFileReader>();
    }
}
