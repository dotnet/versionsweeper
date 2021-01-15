using DotNet.Extensions;
using DotNet.Versions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.VersionSweeper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            var projectReader = host.Services.GetRequiredService<IProjectFileReader>();
            DirectoryInfo directory = new(@"C:\Users\dapine\source\repos\dotnetsamples");
            ConcurrentDictionary<string, string[]> projects = new(StringComparer.OrdinalIgnoreCase);

            await directory.EnumerateFiles("*.csproj", SearchOption.AllDirectories)
                .ForEachAsync(
                    Environment.ProcessorCount,
                    async fileInfo =>
                    {
                        var path = fileInfo.FullName;
                        var tfms = await projectReader.ReadProjectTfmsAsync(path);
                        if (tfms is { Length: > 0 })
                        {
                            projects.TryAdd(path, tfms);
                        }
                    });

            if (projects is { IsEmpty: false })
            {
                var unsupportedProjectReporter =
                    host.Services.GetRequiredService<IUnsupportedProjectReporter>();
                foreach (var (projectPath, tfms) in projects)
                {
                    await foreach (var projectSupportReport
                        in unsupportedProjectReporter.ReportAsync(projectPath, tfms))
                    {
                        var (proj, reports) = projectSupportReport;
                        if (reports is { Count: > 0 })
                        {
                            foreach (var report in reports.Where(r => r.IsUnsupported))
                            {
                                Console.WriteLine(
                                    $"{report.Version} ({report.TargetFrameworkMoniker}) is unsupported, in {proj}");
                            }
                        }
                    }
                }
            }

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) => services.AddDotNetVersionServices());
    }
}
