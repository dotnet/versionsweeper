// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.VersionSweeper;

public static class ServiceProviderExtensions
{
    public static (T1, T2) GetRequiredServices<T1, T2>(
        this IServiceProvider provider)
        where T1 : notnull
        where T2 : notnull =>
        (
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>()
        );

    public static (T1, T2, T3, T4) GetRequiredServices<T1, T2, T3, T4>(
        this IServiceProvider provider)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull =>
        (
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>(),
            provider.GetRequiredService<T3>(),
            provider.GetRequiredService<T4>()
        );
}
