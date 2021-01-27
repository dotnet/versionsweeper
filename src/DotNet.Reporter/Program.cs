using DotNet.Releases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((_, logging) => logging.ClearProviders())
    .ConfigureServices((_, services) => services.AddDotNetReleaseServices())
    .Build();

if (args is { Length: 1 })
{
    var projectPath = args[0];
    if (projectPath is { Length: > 0 })
    {
        T Get<T>() => host.Services.GetRequiredService<T>();

        var projectReader = Get<IProjectFileReader>();
        var (lineNumber, tfms) = await projectReader.ReadProjectTfmsAsync(projectPath);
        if (tfms is { Length: > 0 })
        {
            var reporter = Get<IUnsupportedProjectReporter>();
            await foreach (var projectSupportReport in reporter.ReportAsync(projectPath, tfms))
            {
                var (proj, reports) = projectSupportReport;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    Console.WriteLine($"{projectPath} targets unsupported version(s).");
                    foreach (var report in reports)
                    {
                        Console.WriteLine(
                            $"Line number {lineNumber} targets '{report.TargetFrameworkMoniker}' which is unsupported. " +
                            $"The next nearest LTS (or current) version is '{report.NearestLtsVersion}'");
                    }

                    return 1;
                }
            }
        }
    }
}

return 0;