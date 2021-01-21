using CommandLine;
using DotNet.Extensions;
using DotNet.GitHub;
using DotNet.Versions;
using DotNet.VersionSweeper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CommandLine.Parser;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (_, services) =>
            services.AddDotNetGitHubServices()
                .AddDotNetVersionServices())
    .Build();

var parsedArgs = Default.ParseArguments<Options>(args);
if (parsedArgs.Tag == ParserResultType.Parsed)
{
    await parsedArgs.MapResult(async options =>
    {
        var projectReader = host.Services.GetRequiredService<IProjectFileReader>();
        DirectoryInfo directory = new(options.Directory);
        ConcurrentDictionary<string, string[]> projects = new(StringComparer.OrdinalIgnoreCase);

        await directory.EnumerateFiles(options.SearchPattern, SearchOption.AllDirectories)
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

            var gitHubIssueService =
                host.Services.GetRequiredService<IGitHubIssueService>();

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
                            var issue = await gitHubIssueService.PostIssueAsync(
                                options.Owner, options.Name, options.Token,
                                new("Title")
                                {
                                    Body = ""
                                });

                            Console.WriteLine(
                                $"{report.Version} ({report.TargetFrameworkMoniker}) is unsupported, in {proj}");
                        }
                    }
                }
            }
        }
    },
    errors =>
    {
        foreach (var error in errors)
        {
            Console.WriteLine(error);
        }
        return Task.CompletedTask;
    });
}

await host.RunAsync();
