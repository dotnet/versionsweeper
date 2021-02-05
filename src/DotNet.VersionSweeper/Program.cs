using CommandLine;
using DotNet.GitHub;
using DotNet.GitHubActions;
using DotNet.Models;
using DotNet.Releases;
using DotNet.VersionSweeper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit;
using System;
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

static async Task StartSweeperAsync(Options options, IServiceProvider services, IJobService job)
{
    try
    {
        options.WriteDebugInfo(job);

        var (solutions, orphanedProjects) =
            await Discovery.FindSolutionsAndProjectsAsync(services, job, options);

        var (unsupportedProjectReporter, issueQueue, graphQLClient) =
                services.GetRequiredServices
                    <IUnsupportedProjectReporter, RateLimitAwareQueue, GitHubGraphQLClient>();

        foreach (var solution in solutions.Where(sln => sln is not null))
        {
            SolutionSupportReport solutionSupportReport = new(solution);

            foreach (var project in solution.Projects)
            {
                await foreach (var projectSupportReport
                    in unsupportedProjectReporter.ReportAsync(project))
                {
                    solutionSupportReport.ProjectSupportReports.Add(projectSupportReport);
                }
            }

            var reports = solutionSupportReport.ProjectSupportReports;

            if (reports is { Count: > 0 } &&
                reports.Any(r => r.TargetFrameworkMonikerSupports.Any(s => s.IsUnsupported)))
            {
                var title = solutionSupportReport.ToTitleMessage();
                var existingIssue =
                    await graphQLClient.GetIssueAsync(
                        options.Owner, options.Name, options.Token, title);
                if (existingIssue?.State == ItemState.Open)
                {
                    job.Info($"Re-discovered but ignoring, latent non-LTS version in {existingIssue}.");
                }
                else
                {
                    issueQueue.Enqueue(
                        new(options.Owner, options.Name, options.Token),
                        new(solutionSupportReport.ToTitleMessage())
                        {
                            Body = solutionSupportReport.ToMarkdownBody(
                                    options.Directory, options.Branch)
                        });
                }
            }
        }

        foreach (var orphanedProject in orphanedProjects)
        {
            await foreach (var projectSupportReport
                in unsupportedProjectReporter.ReportAsync(orphanedProject))
            {
                var (project, reports) = projectSupportReport;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    var title = projectSupportReport.ToTitleMessage();
                    var existingIssue =
                        await graphQLClient.GetIssueAsync(
                            options.Owner, options.Name, options.Token, title);
                    if (existingIssue?.State == ItemState.Open)
                    {
                        job.Info($"Re-discovered but ignoring, latent non-LTS version in {existingIssue}.");
                    }
                    else
                    {
                        issueQueue.Enqueue(
                            new(options.Owner, options.Name, options.Token),
                            new(projectSupportReport.ToTitleMessage())
                            {
                                Body = projectSupportReport.ToMarkdownBody(
                                        options.Directory, options.Branch)
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
