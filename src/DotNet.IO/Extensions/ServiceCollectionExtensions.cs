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
