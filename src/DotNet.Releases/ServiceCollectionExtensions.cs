using DotNet.Versions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotNetVersionServices(
            this IServiceCollection services) =>
            services
                .AddMemoryCache()
                .AddHttpClient()
                .AddSingleton<ReleaseIndexService>()
                .AddSingleton<ReleaseService>();
                
    }
}
