using CommandLine;
using DotNet.Extensions;
using DotNet.GitHub;
using DotNet.GitHubActions;
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
                .AddGitHubActionServices()
                .AddDotNetReleaseServices())
    .Build();

var jobService =
    host.Services.GetRequiredService<IJobService>();
var parser =
    Default.ParseArguments<Options>(() => new(), args);

static void ReportOptionsAndDebugInfo(Options options, IJobService job)
{
    job.SetCommandEcho(true);

    job.Info($"repository owner: {options.Owner}");
    job.Info($"repository name: {options.Name}");
    job.Info($"current branch: {options.Branch}");
    job.Info($"root directory to search: {options.Directory}");
    var parsedPatterns = string.Join(", ",
        options.SearchPattern?.AsMaskedExtensions().AsRecursivePatterns() ?? Array.Empty<string>());
    job.Info($"parsed patterns: {parsedPatterns}");
}

static async Task StartSweeperAsync(Options options, IServiceProvider services, IJobService job)
{
    try
    {
        ReportOptionsAndDebugInfo(options, job);

        var projectReader = services.GetRequiredService<IProjectFileReader>();
        DirectoryInfo directory = new(options.Directory!);
        ConcurrentDictionary<string, (int, string[])> projects = new(StringComparer.OrdinalIgnoreCase);

        var config = await VersionSweeperConfig.ReadAsync(options.Directory!, job);
        var matcher = config.Ignore.GetMatcher(options.SearchPattern!);

        await matcher.GetResultsInFullPath(options.Directory!)
            .ForEachAsync(
                Environment.ProcessorCount,
                async path =>
                {
                    var (lineNumber, tfms) = await projectReader.ReadProjectTfmsAsync(path);
                    if (tfms is { Length: > 0 })
                    {
                        if (projects.TryAdd(path, (lineNumber, tfms)))
                        {
                            job.Info($"Parsed TFM(s): '{string.Join(", ", tfms)}' on line {lineNumber} in {path}.");
                        }
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
                        var title = projectSupportReport.ToTitleMessage();
                        var existingIssue =
                            await graphQLClient.GetIssueAsync(
                                options.Owner!, options.Name!, options.Token!, title);
                        if (existingIssue?.State == ItemState.Open)
                        {
                            job.Info($"Re-discovered but ignoring, latent non-LTS version in {existingIssue}.");
                        }
                        else
                        {
                            await issueQueue.EnqueueAsync(
                                new(options.Owner!, options.Name!, options.Token!),
                                new(projectSupportReport.ToTitleMessage())
                                {
                                    Body = projectSupportReport.ToMarkdownBody(
                                            options.Directory!, lineNumber)
                                });
                        }
                    }
                }
            }

            await foreach (var issue in issueQueue.ExecuteAllQueuedItemsAsync())
            {
                job.Info($"Created issue: {issue.HtmlUrl}");
            }
        }
        else
        {
            job.Info($"No projects found matching: {options.SearchPattern}, in '{options.Directory}'.");
        }
    }
    catch (Exception ex)
    {
        job.SetFailed(ex.ToString());
    }
    finally
    {
        Environment.Exit(0);
    }
}

parser.WithNotParsed(
    errors => jobService.SetFailed(
        string.Join(Environment.NewLine, errors.Select(error => error.ToString()))));

await parser.WithParsedAsync(options => StartSweeperAsync(options, host.Services, jobService));
await host.RunAsync();
