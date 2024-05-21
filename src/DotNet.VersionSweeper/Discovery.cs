// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.VersionSweeper;

sealed class Discovery
{
    /// <summary>
    /// Returns a list of solutions, each solution contains projects. Also returns a mapping
    /// of orphaned projects that do not belong to solutions, but match the search patterns.
    /// </summary>
    internal static async Task<(IImmutableSet<Solution> Solutions, IImmutableSet<ModelProject> OrphanedProjects, VersionSweeperConfig Config)>
        FindSolutionsAndProjectsAsync(
            IServiceProvider services,
            ICoreService job,
            Options options)
    {
        (ISolutionFileReader solutionReader, IProjectFileReader projectReader) =
            services.GetRequiredServices<ISolutionFileReader, IProjectFileReader>();

        (DirectoryInfo directory, HashSet<string> extensions) =
            (options.ToDirectoryInfo(), options.AsFileExtensions());

        ConcurrentBag<Solution> solutions = [];
        ConcurrentBag<ModelProject> projects = [];

        VersionSweeperConfig config = await VersionSweeperConfig.ReadAsync(options.Directory, job);

        Matcher solutionMatcher = new Matcher().AddInclude("**/*.sln");
        Matcher projectMatcher = options.AsGlobMatcher(config.Ignore);

        await Task.WhenAll(
            projectMatcher.GetResultsInFullPath(options.Directory)
                .ForEachAsync(
                    ProcessorCount,
                    async path =>
                    {
                        ModelProject project = await projectReader.ReadProjectAsync(path);
                        if (project is { TfmLineNumber: > -1 })
                        {
                            projects.Add(project);
                            job.WriteInfo($"Parsed TFM(s): '{string.Join(", ", project.Tfms)}' on line {project.TfmLineNumber} in {path}.");
                        }
                    }),
            solutionMatcher.GetResultsInFullPath(options.Directory)
                .ForEachAsync(
                    ProcessorCount,
                    async path =>
                    {
                        Solution? solution = await solutionReader.ReadSolutionAsync(path);
                        if (solution is not null && solution.Projects is not null)
                        {
                            PatternMatchingResult match = projectMatcher.Match(
                                options.Directory,
                                solution.Projects.Select(proj => proj.FullPath));

                            if (match.HasMatches)
                            {
                                var matchingProjects =
                                    match.Files
                                        .Select(f => Path.GetFullPath(Path.Combine(options.Directory, f.Path)))
                                        .Distinct()
                                        .ToHashSet();

                                solution.Projects.RemoveWhere(
                                    proj => !matchingProjects.Contains(proj.FullPath));

                                if (solution.Projects.Count > 0)
                                {
                                    job.WriteInfo($"Read solution with {solution.Projects.Count} projects in it.");
                                    solutions.Add(solution);
                                }
                            }
                        }
                    })
            );

        var solutionSet = solutions.ToImmutableHashSet();
        var orphanedProjectSet =
            projects.Except(solutions.SelectMany(sln => sln.Projects))
                .ToImmutableHashSet();

        job.WriteInfo($"Discovered {solutionSet.Count} solutions and {orphanedProjectSet.Count} orphaned projects.");

        return
            (
                Solutions: solutionSet,
                OrphanedProjects: orphanedProjectSet,
                Config: config
            );
    }

    internal static async Task<IImmutableSet<Dockerfile>> FindDockerfilesAsync(
        IServiceProvider services,
        ICoreService job,
        Options options)
    {
        var dockerfileReader = services.GetRequiredService<IDockerfileReader>();
        ConcurrentBag<Dockerfile> dockerfiles = [];
        var dockerfileMatcher = new Matcher().AddInclude("**/Dockerfile");

        await dockerfileMatcher.GetResultsInFullPath(options.Directory)
            .ForEachAsync(
                ProcessorCount,
                async path =>
                {
                    Dockerfile dockerfile = await dockerfileReader.ReadDockerfileAsync(path);
                    if (dockerfile is { ImageDetails.Count: > 0 })
                    {
                        dockerfiles.Add(dockerfile);
                        foreach (ImageDetails imageDetail in dockerfile.ImageDetails)
                        {
                            job.WriteInfo($"Parsed TFM(s): '{imageDetail.TargetFrameworkMoniker}' on line {imageDetail.LineNumber} in {path}.");
                        }
                    }
                });

        return dockerfiles.ToImmutableHashSet();
    }
}
