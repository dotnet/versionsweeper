// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

        var (solutions, orphanedProjects, config) =
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
            var (isError, existingIssue) =
                await client.GetIssueAsync(
                    options.Owner, options.Name, options.Token, title);
            if (isError)
            {
                job.Debug($"Error checking for existing issue, best not to create an issue as it may be a duplicate.");
            }
            else if (existingIssue is { State: ItemState.Open })
            {
                var markdownBody = getBody(options);
                if (markdownBody != existingIssue.Body)
                {
                    // These updates will overwrite completed tasks in a check list
                    // They'll be removed when the issue updated.
                    queue.Enqueue(
                        new(options.Owner, options.Name, options.Token, existingIssue.Number),
                        new IssueUpdate
                        {
                            Body = markdownBody
                        });
                }
                else
                {
                    job.Info($"Re-discovered but ignoring, latent issue: {existingIssue}.");
                }
            }
            else
            {
                var markdownBody = getBody(options);
                queue.Enqueue(
                    new(options.Owner, options.Name, options.Token),
                    new NewIssue(title)
                    {
                        Body = markdownBody
                    });
            }
        }

        HashSet<ModelProject> nonSdkStyleProjects = new();
        Dictionary<string, HashSet<ProjectSupportReport>> tfmToProjectSupportReports =
            new(StringComparer.OrdinalIgnoreCase);

        void AppendGrouping(
            IGrouping<string, (TargetFrameworkMonikerSupport tfms, ProjectSupportReport psr)> grouping)
        {
            var key = grouping.Key;
            if (!tfmToProjectSupportReports.ContainsKey(key))
            {
                tfmToProjectSupportReports[key] = new();
            }

            foreach (var group in grouping)
            {
                tfmToProjectSupportReports[key].Add(group.psr);
            }
        }

        foreach (var solution in solutions.Where(sln => sln is not null))
        {
            SolutionSupportReport solutionSupportReport = new(solution);

            foreach (var project in solution.Projects)
            {
                if (!project.IsSdkStyle)
                {
                    nonSdkStyleProjects.Add(project);
                }

                await foreach (var psr in unsupportedProjectReporter.ReportAsync(
                    project, config.OutOfSupportWithinDays))
                {
                    solutionSupportReport.ProjectSupportReports.Add(psr);
                }
            }

            var reports = solutionSupportReport.ProjectSupportReports;
            if (reports is { Count: > 0 } &&
                reports.Any(r => r.TargetFrameworkMonikerSupports.Any(s => s.IsUnsupported)))
            {
                foreach (var grouping in
                    reports.Where(r => r.TargetFrameworkMonikerSupports.Any(s => s.IsUnsupported))
                        .SelectMany(
                            psr => psr.TargetFrameworkMonikerSupports, (psr, tfms) => (tfms, psr))
                        .GroupBy(t => t.tfms.TargetFrameworkMoniker))
                {
                    AppendGrouping(grouping);
                }
            }
        }

        foreach (var orphanedProject in orphanedProjects)
        {
            if (!orphanedProject.IsSdkStyle)
            {
                nonSdkStyleProjects.Add(orphanedProject);
            }

            await foreach (var psr in unsupportedProjectReporter.ReportAsync(
                orphanedProject, config.OutOfSupportWithinDays))
            {
                var (project, reports) = psr;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    foreach (var grouping in
                        reports.Select(tfms => (tfms, psr))
                            .GroupBy(t => t.tfms.TargetFrameworkMoniker))
                    {
                        AppendGrouping(grouping);
                    }
                }
            }
        }

        foreach (var (tfm, projectSupportReports) in tfmToProjectSupportReports)
        {
            await CreateAndEnqueueAsync(
                graphQLClient, issueQueue, job,
                $"Upgrade from `{tfm}` to LTS (or current) version",
                options, o => projectSupportReports.ToMarkdownBody(tfm, o.Directory, o.Branch));
        }

        if (nonSdkStyleProjects.TryCreateIssueContent(
            options.Directory, options.Branch, out var content))
        {
            var (title, markdownBody) = content;
            await CreateAndEnqueueAsync(
                graphQLClient, issueQueue, job, title, options, _ => markdownBody);
        }

        await foreach (var (type, issue) in issueQueue.ExecuteAllQueuedItemsAsync())
        {
            job.Info($"{type} issue: {issue.HtmlUrl}");
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
