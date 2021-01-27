using CommandLine;
using DotNet.Extensions;
using DotNet.GitHub;
using DotNet.Releases;
using DotNet.VersionSweeper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Octokit;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CommandLine.Parser;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddDotNetGitHubServices()
                .AddDotNetReleaseServices())
    .Build();

var parser =
    Default.ParseArguments<Options>(
        args.OverrideFromEnvironmentVariables());

async Task StartSweeperAsync(Options options, IServiceProvider services)
{
    var projectReader = services.GetRequiredService<IProjectFileReader>();
    DirectoryInfo directory = new(options.Directory);
    ConcurrentDictionary<string, (int, string[])> projects = new(StringComparer.OrdinalIgnoreCase);

    var config = await VersionSweeperConfig.ReadAsync(options.Directory);
    var matcher = config.Ignore.GetMatcher(options.SearchPattern);

    await matcher.GetResultsInFullPath(options.Directory)
        .ForEachAsync(
            Environment.ProcessorCount,
            async path =>
            {
                var (lineNumber, tfms) = await projectReader.ReadProjectTfmsAsync(path);
                if (tfms is { Length: > 0 })
                {
                    projects.TryAdd(path, (lineNumber, tfms));
                }
            });

    if (projects is not { IsEmpty: true })
    {
        var (unsupportedProjectReporter, issueQueue, graphQLClient) =
           services.GetRequiredServices
               <IUnsupportedProjectReporter, RateLimitAwareQueue, GitHubGraphQLClient>();

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
                            await issueQueue.EnqueueAsync(
                                new(options.Owner, options.Name, options.Token),
                                new(projectSupportReport.ToTitleMessage())
                                {
                                    Body = projectSupportReport.ToMarkdownBody(
                                            options.Directory, lineNumber)
                                });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }
            }
        }

        try
        {
            await foreach (var _ in issueQueue.ExecuteAllQueuedItemsAsync()) { }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }
}

parser.WithNotParsed(errors => errors.ForEach(Console.WriteLine));

await parser.WithParsedAsync(options => StartSweeperAsync(options, host.Services));
await host.RunAsync();
