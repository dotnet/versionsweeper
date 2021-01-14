using DotNet.Versions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DotNet.VersionSweeper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            var service = host.Services.GetRequiredService<ReleaseService>();
            await foreach (var release in service.GetAllReleasesAsync())
            {
                Console.WriteLine($"Version: {release}");
                Console.WriteLine();
            }

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) => services.AddDotNetVersionServices());
    }
}
