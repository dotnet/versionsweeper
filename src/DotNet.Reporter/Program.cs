// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.Releases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((_, logging) => logging.ClearProviders())
    .ConfigureServices((_, services) => services.AddDotNetReleaseServices().AddDotNetFileSystem())
    .Build();

if (args is { Length: 1 })
{
    string projectPath = args[0];
    if (projectPath is { Length: > 0 })
    {
        T Get<T>() => host.Services.GetRequiredService<T>();

        IProjectFileReader projectReader = Get<IProjectFileReader>();
        DotNet.Models.Project project = await projectReader.ReadProjectAsync(projectPath);
        if (project is { TfmLineNumber: > -1 })
        {
            IUnsupportedProjectReporter reporter = Get<IUnsupportedProjectReporter>();
            await foreach (DotNet.Models.ProjectSupportReport projectSupportReport in reporter.ReportAsync(project, 0))
            {
                (DotNet.Models.Project proj, HashSet<DotNet.Models.TargetFrameworkMonikerSupport> reports) = projectSupportReport;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    Console.WriteLine($"{projectPath} targets unsupported version(s).");
                    foreach (DotNet.Models.TargetFrameworkMonikerSupport report in reports)
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
