using DotNet.Releases;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotNetVersionServices(
            this IServiceCollection services) =>
            services
                .AddMemoryCache()
                .AddHttpClient()
                .AddSingleton<ICoreReleaseIndexService, CoreReleaseIndexService>()
                .AddSingleton<ICoreReleaseService, CoreReleaseService>()
                .AddSingleton<IFrameworkReleaseIndexService, FrameworkReleaseIndexService>()
                .AddSingleton<IFrameworkReleaseService, FrameworkReleaseService>()
                .AddSingleton<IProjectFileReader, ProjectFileReader>()
                .AddSingleton<IUnsupportedProjectReporter, UnsupportedProjectReporter>();
    }
}
