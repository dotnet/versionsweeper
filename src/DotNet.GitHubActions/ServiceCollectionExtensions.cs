using Microsoft.Extensions.DependencyInjection;

namespace DotNet.GitHubActions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGitHubActionServices(
            this IServiceCollection services) => services.AddSingleton<IJobService, JobService>();
    }
}
