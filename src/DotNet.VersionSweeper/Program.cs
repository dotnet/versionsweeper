// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using DotNet.GitHub;
using DotNet.GitHubActions;
using DotNet.Models;
using DotNet.Releases;
using DotNet.VersionSweeper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit;
using static CommandLine.Parser;
using Project = DotNet.Models.Project;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddDotNetGitHubServices()
                .AddGitHubActionServices()
                .AddDotNetReleaseServices()
                .AddDotNetFileSystem())
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

        static async Task CreateAndEnqueueAsync(
            GitHubGraphQLClient client,
            RateLimitAwareQueue queue,
            IJobService job,
            string title, Options options, Func<Options, string> getBody)
        {
            var existingIssue =
                await client.GetIssueAsync(
                    options.Owner, options.Name, options.Token, title);
            if (existingIssue is null)
            {
                job.Debug($"Error checking for existing issue, best not to create an issue as it may be a duplicate.");
            }
            else if (existingIssue is { State: ItemState.Open })
            {
                job.Info($"Re-discovered but ignoring, latent issue: {existingIssue}.");
            }
            else
            {
                var markdownBody = getBody(options);
                queue.Enqueue(
                    new(options.Owner, options.Name, options.Token),
                    new(title)
                    {
                        Body = markdownBody
                    });
            }
        }

        HashSet<Project> nonSdkStyleProjects = new();

        foreach (var solution in solutions.Where(sln => sln is not null))
        {
            SolutionSupportReport solutionSupportReport = new(solution);

            foreach (var project in solution.Projects)
            {
                if (!project.IsSdkStyle)
                {
                    nonSdkStyleProjects.Add(project);
                }

                await foreach (var psr in unsupportedProjectReporter.ReportAsync(project))
                {
                    solutionSupportReport.ProjectSupportReports.Add(psr);
                }
            }

            var reports = solutionSupportReport.ProjectSupportReports;
            if (reports is { Count: > 0 } &&
                reports.Any(r => r.TargetFrameworkMonikerSupports.Any(s => s.IsUnsupported)))
            {
                await CreateAndEnqueueAsync(
                    graphQLClient, issueQueue, job,
                    solutionSupportReport.ToTitleMessage(),
                    options, o => solutionSupportReport.ToMarkdownBody(o.Directory, o.Branch));
            }
        }

        foreach (var orphanedProject in orphanedProjects)
        {
            if (!orphanedProject.IsSdkStyle)
            {
                nonSdkStyleProjects.Add(orphanedProject);
            }

            await foreach (var psr in unsupportedProjectReporter.ReportAsync(orphanedProject))
            {
                var (project, reports) = psr;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    await CreateAndEnqueueAsync(
                        graphQLClient, issueQueue, job,
                        psr.ToTitleMessage(),
                        options, o => psr.ToMarkdownBody(o.Directory, o.Branch));
                }
            }
        }

        if (nonSdkStyleProjects.TryCreateIssueContent(
            options.Directory, options.Branch, out var content))
        {
            var (title, markdownBody) = content;
            await CreateAndEnqueueAsync(
                graphQLClient, issueQueue, job, title, options, _ => markdownBody);
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
