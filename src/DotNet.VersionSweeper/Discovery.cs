using DotNet.Extensions;
using DotNet.GitHubActions;
using DotNet.Models;
using DotNet.Releases;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.VersionSweeper
{
    class Discovery
    {
        /// <summary>
        /// Returns a list of solutions, each solution contains projects. Also returns a mapping
        /// of orphaned
        /// </summary>
        internal static async Task<(ISet<Solution> Solutions, ISet<Project> OrphanedProjects)>
            FindSolutionsAndProjectsAsync(
                IServiceProvider services,
                IJobService job,
                Options options)
        {
            var (solutionReader, projectReader) =
                services.GetRequiredServices<ISolutionFileReader, IProjectFileReader>();

            var (directory, extensions) =
                (options.ToDirectoryInfo(), options.AsFileExtensions());

            ConcurrentBag<Solution> solutions = new();
            ConcurrentBag<Project> projects = new();

            var config = await VersionSweeperConfig.ReadAsync(options.Directory, job);

            var solutionMatcher = new Matcher().AddInclude("**/*.sln");
            var projectMatcher = options.AsGlobMatcher(config.Ignore);

            await Task.WhenAll(
                projectMatcher.GetResultsInFullPath(options.Directory)
                    .ForEachAsync(
                        Environment.ProcessorCount,
                        async path =>
                        {
                            var project = await projectReader.ReadProjectAsync(path);
                            if (project is { TfmLineNumber: > -1 })
                            {
                                projects.Add(project);
                                job.Info($"Parsed TFM(s): '{string.Join(", ", project.Tfms)}' on line {project.TfmLineNumber} in {path}.");
                            }
                        }),
                solutionMatcher.GetResultsInFullPath(options.Directory)
                    .ForEachAsync(
                        Environment.ProcessorCount,
                        async path =>
                        {
                            var solution = await solutionReader.ReadSolutionAsync(path);
                            if (solution is not null && solution.Projects is not null)
                            {
                                if (solution.Projects.Any(proj => extensions.Contains(proj.Extension)))
                                {
                                    job.Info($"Read solution with {solution.Projects.Count} projects in it.");
                                    solutions.Add(solution);
                                }
                            }
                        })
                );


            return
                (
                    Solutions: solutions.ToHashSet(),
                    OrphanedProjects:
                        projects.Except(solutions.SelectMany(sln => sln.Projects))
                            .ToHashSet()
                );
        }
    }
}
