// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using DotNet.IO;
using DotNet.Releases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((_, logging) => logging.ClearProviders())
    .ConfigureServices((_, services) => services.AddDotNetReleaseServices().AddDotNetFileSystem())
    .Build();

if (args is { Length: 1 })
{
    var projectPath = args[0];
    if (projectPath is { Length: > 0 })
    {
        T Get<T>() => host.Services.GetRequiredService<T>();

        var projectReader = Get<IProjectFileReader>();
        var project = await projectReader.ReadProjectAsync(projectPath);
        if (project is { TfmLineNumber: > -1 })
        {
            var reporter = Get<IUnsupportedProjectReporter>();
            await foreach (var projectSupportReport in reporter.ReportAsync(project, 0))
            {
                var (proj, reports) = projectSupportReport;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    Console.WriteLine($"{projectPath} targets unsupported version(s).");
                    foreach (var report in reports)
                    {
                        Console.WriteLine(
                            $"Line number {project.TfmLineNumber} targets '{report.TargetFrameworkMoniker}' which is unsupported. " +
                            $"The next nearest LTS (or current) version is '{report.NearestLtsVersion}'");
                    }

                    return 1;
                }
            }
        }
    }
}

return 0;
