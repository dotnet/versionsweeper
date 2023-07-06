// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDotNetGitHubServices();
builder.Services.AddGitHubActionsCore();
builder.Services.AddDotNetReleaseServices();
builder.Services.AddDotNetFileSystem();

using IHost host = builder.Build();

ICoreService jobService =
    host.Services.GetRequiredService<ICoreService>();
ParserResult<Options> parser =
    Default.ParseArguments<Options>(() => new(), args);

parser.WithNotParsed(
    errors => jobService.SetFailed(
        string.Join(NewLine, errors.Select(error => error.ToString()))));

await parser.WithParsedAsync(options => StartSweeperAsync(options, host.Services, jobService));
await host.RunAsync();

static async Task StartSweeperAsync(Options options, IServiceProvider services, ICoreService job)
{
    try
    {
        options.WriteDebugInfo(job);

        (IImmutableSet<Solution> solutions, IImmutableSet<ModelProject> orphanedProjects, VersionSweeperConfig config) =
            await Discovery.FindSolutionsAndProjectsAsync(services, job, options);

        IImmutableSet<Dockerfile> dockerfiles =
            await Discovery.FindDockerfilesAsync(services, job, options);

        (IUnsupportedProjectReporter unsupportedProjectReporter, IUnsupportedDockerfileReporter unsupportedDockerfileReporter, RateLimitAwareQueue queue, GitHubGraphQLClient graphQLClient) =
                services.GetRequiredServices
                    <IUnsupportedProjectReporter, IUnsupportedDockerfileReporter,
                        RateLimitAwareQueue, GitHubGraphQLClient>();

        HashSet<ModelProject> nonSdkStyleProjects = new();
        Dictionary<string, HashSet<ProjectSupportReport>> tfmToProjectSupportReports =
            new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, HashSet<DockerfileSupportReport>> tfmToDockerfileSupportReports =
            new(StringComparer.OrdinalIgnoreCase);

        // Work through all solutions, creating support reports.
        foreach (Solution? solution in solutions.Where(sln => sln is not null))
        {
            SolutionSupportReport solutionSupportReport = new(solution);

            foreach (ModelProject project in solution.Projects)
            {
                if (!project.IsSdkStyle)
                {
                    nonSdkStyleProjects.Add(project);
                }

                await foreach (ProjectSupportReport psr in unsupportedProjectReporter.ReportAsync(
                    project, config.OutOfSupportWithinDays))
                {
                    solutionSupportReport.ProjectSupportReports.Add(psr);
                }
            }

            HashSet<ProjectSupportReport> reports = solutionSupportReport.ProjectSupportReports;
            if (reports is { Count: > 0 } &&
                reports.Any(r => r.TargetFrameworkMonikerSupports.Any(s => s.IsUnsupported)))
            {
                foreach (IGrouping<string, (TargetFrameworkMonikerSupport tfms, ProjectSupportReport psr)> grouping in
                    reports.Where(r => r.TargetFrameworkMonikerSupports.Any(s => s.IsUnsupported))
                        .SelectMany(
                            psr => psr.TargetFrameworkMonikerSupports, (psr, tfms) => (tfms, psr))
                        .GroupBy(t => t.tfms.TargetFrameworkMoniker))
                {
                    AppendGrouping(tfmToProjectSupportReports, grouping);
                }
            }
        }

        // Work through any orphaned projects, creating support reports.
        foreach (ModelProject orphanedProject in orphanedProjects)
        {
            if (!orphanedProject.IsSdkStyle)
            {
                nonSdkStyleProjects.Add(orphanedProject);
            }

            await foreach (ProjectSupportReport psr in unsupportedProjectReporter.ReportAsync(
                orphanedProject, config.OutOfSupportWithinDays))
            {
                (ModelProject project, HashSet<TargetFrameworkMonikerSupport> reports) = psr;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    foreach (IGrouping<string, (TargetFrameworkMonikerSupport tfms, ProjectSupportReport psr)> grouping in
                        reports.Select(tfms => (tfms, psr))
                            .GroupBy(t => t.tfms.TargetFrameworkMoniker))
                    {
                        AppendGrouping(tfmToProjectSupportReports, grouping);
                    }
                }
            }
        }

        // Work through all dockerfiles, creating support reports.
        foreach (Dockerfile dockerfile in dockerfiles)
        {
            await foreach (DockerfileSupportReport supportReport in unsupportedDockerfileReporter.ReportAsync(
                dockerfile, config.OutOfSupportWithinDays))
            {
                HashSet<TargetFrameworkMonikerSupport> reports = supportReport.TargetFrameworkMonikerSupports;
                if (reports is { Count: > 0 } && reports.Any(r => r.IsUnsupported))
                {
                    foreach (IGrouping<string, (TargetFrameworkMonikerSupport tfms, DockerfileSupportReport supportReport)> grouping in
                        reports.Select(tfms => (tfms, supportReport))
                            .GroupBy(t => t.tfms.TargetFrameworkMoniker))
                    {
                        AppendGrouping(tfmToDockerfileSupportReports, grouping);
                    }
                }
            }
        }

        bool hasRemainingWork = false;

        // Only create issues when explicitly instructed.
        if (config.ActionType is not ActionType.PullRequest)
        {
            // Queue work for Dockerfile TFMs that are out of support.
            foreach ((string tfm, HashSet<DockerfileSupportReport> dockerfileSupportReports) in tfmToDockerfileSupportReports)
            {
                await CreateAndEnqueueAsync(
                    graphQLClient, queue, job,
                    $"Upgrade from `{tfm}` to LTS (or STS) image tag",
                    options, o => dockerfileSupportReports.ToMarkdownBody(tfm, o));
            }

            // Queue work for project TFMs that are out of support.
            foreach ((string tfm, HashSet<ProjectSupportReport> projectSupportReports) in tfmToProjectSupportReports)
            {
                await CreateAndEnqueueAsync(
                    graphQLClient, queue, job,
                    $"Upgrade from `{tfm}` to LTS (or STS) version",
                    options, o => projectSupportReports.ToMarkdownBody(tfm, o));
            }
        }
        else // We were instructed to create pull requests.
        {
            string[] upgradeProjects = 
                tfmToProjectSupportReports.Values
                    .SelectMany(
                        static reports => reports.Select(
                            static report => report.Project.FullPath))
                    .Distinct()
                    .ToArray();

            // Output that we have remaining work, and the projects to upgrade.
            hasRemainingWork = upgradeProjects is { Length: > 0 };
            if (hasRemainingWork)
            {
                await job.SetOutputAsync("upgrade-projects", upgradeProjects);
            }
        }

        // Non-SDK projects are bundled into a single issue.
        if (config.ActionType is not ActionType.PullRequest &&
            nonSdkStyleProjects.TryCreateIssueContent(
            options.Directory, options.Branch, out (string Title, string MarkdownBody) content))
        {
            (string title, string markdownBody) = content;
            await CreateAndEnqueueAsync(
                graphQLClient, queue, job, title, options, _ => markdownBody);
        }

        // All queued items are executed with the rate-limit aware service.
        await foreach ((string message, string url) in queue.ExecuteAllQueuedItemsAsync())
        {
            job.Info($"{message}: {url}");
        }

        await job.SetOutputAsync("has-remaining-work", hasRemainingWork);
    }
    catch (Exception ex)
    {
        job.SetFailed(ex.ToString());
    }
    finally
    {
        Exit(0);
    }
}

static async Task CreateAndEnqueueAsync(
    GitHubGraphQLClient client,
    RateLimitAwareQueue queue,
    ICoreService job,
    string title,
    Options options,
    Func<Options, string> getBody)
{
    (bool isError, ExistingIssue? existingIssue) =
        await client.GetIssueAsync(
            options.Owner, options.Name, options.Token, title);
    if (isError)
    {
        job.Debug($"Error checking for existing issue, best not to create an issue as it may be a duplicate.");
    }
    else if (existingIssue is { State: ItemState.Open })
    {
        string markdownBody = getBody(options);
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
        string markdownBody = getBody(options);
        queue.Enqueue(
            new(options.Owner, options.Name, options.Token),
            new NewIssue(title)
            {
                Body = markdownBody
            });
    }
}

static void AppendGrouping<T>(
    Dictionary<string, HashSet<T>> tfmToSupportReport,
    IGrouping<string, (TargetFrameworkMonikerSupport tfms, T report)> grouping)
{
    string key = grouping.Key;
    if (!tfmToSupportReport.TryGetValue(key, out HashSet<T>? value))
    {
        value = new();
        tfmToSupportReport[key] = value;
    }

    foreach ((TargetFrameworkMonikerSupport _, T report) in grouping)
    {
        value.Add(report);
    }
}
