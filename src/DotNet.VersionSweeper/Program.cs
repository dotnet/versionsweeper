using CommandLine;
using DotNet.Extensions;
using DotNet.GitHub;
using DotNet.Releases;
using DotNet.VersionSweeper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CommandLine.Parser;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddDotNetGitHubServices()
                .AddDotNetVersionServices())
    .Build();

var parser =
    Default.ParseArguments<Options>(
        args.OverrideFromEnvironmentVariables());

static void HandleParseError(IEnumerable<Error> errors)
{
    foreach (var error in errors)
    {
        Console.WriteLine(error);
    }
}

async Task StartSweeperAsync(Options options, IServiceProvider services)
{
    var projectReader = services.GetRequiredService<IProjectFileReader>();
    DirectoryInfo directory = new(options.Directory);
    ConcurrentDictionary<string, (int, string[])> projects = new(StringComparer.OrdinalIgnoreCase);

    // TODO: Add support for multiple file extensions.
    // i.e.; SearchPattern = "*.csproj|*.fsproj|*.vbproj"
    await directory.EnumerateFiles(options.SearchPattern, SearchOption.AllDirectories)
        .ForEachAsync(
            Environment.ProcessorCount,
            async fileInfo =>
            {
                var path = fileInfo.FullName;
                var (lineNumber, tfms) = await projectReader.ReadProjectTfmsAsync(path);
                if (tfms is { Length: > 0 })
                {
                    projects.TryAdd(path, (lineNumber, tfms));
                }
            });

    if (projects is not { IsEmpty: true })
    {
        var unsupportedProjectReporter =
            services.GetRequiredService<IUnsupportedProjectReporter>();
        var gitHubIssueService =
            services.GetRequiredService<IGitHubIssueService>();
        var graphQLClient =
            services.GetRequiredService<GitHubGraphQLClient>();

        foreach (var (projectPath, (lineNumber, tfms)) in projects)
        {
            await foreach (var projectSupportReport
                in unsupportedProjectReporter.ReportAsync(projectPath, tfms))
            {
                var (proj, reports) = projectSupportReport;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    try
                    {
                        var title = projectSupportReport.ToTitleMessage();
                        var existingIssue =
                            await graphQLClient.GetIssueAsync(
                                options.Owner, options.Name, options.Token, title);
                        if (existingIssue?.State == ItemState.Open)
                        {
                            Console.WriteLine(existingIssue);
                        }
                        else
                        {
                            var issue =
                                await gitHubIssueService.PostIssueAsync(
                                    options.Owner,
                                    options.Name,
                                    options.Token,
                                    new(projectSupportReport.ToTitleMessage())
                                    {
                                        Body = projectSupportReport.ToMarkdownBody(
                                            options.Directory, lineNumber)
                                    });

                            Console.WriteLine($"{issue.HtmlUrl}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }
            }
        }
    }
}

parser.WithNotParsed(HandleParseError);

await parser.WithParsedAsync(options => StartSweeperAsync(options, host.Services));
await host.RunAsync();
