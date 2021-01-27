using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.VersionSweeper
{
    public static class ServiceProviderExtensions
    {
        public static (T1, T2, T3) GetRequiredServices<T1, T2, T3>(
            this IServiceProvider provider)
            where T1 : notnull
            where T2 : notnull
            where T3 : notnull =>
            (
                provider.GetRequiredService<T1>(),
                provider.GetRequiredService<T2>(),
                provider.GetRequiredService<T3>()
            );
    }
}
