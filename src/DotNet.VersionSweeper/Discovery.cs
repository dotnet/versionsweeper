﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Extensions;
using DotNet.GitHubActions;
using DotNet.IO;
using DotNet.Models;
using Microsoft.Extensions.FileSystemGlobbing;

namespace DotNet.VersionSweeper
{
    class Discovery
    {
        /// <summary>
        /// Returns a list of solutions, each solution contains projects. Also returns a mapping
        /// of orphaned projects that do not belong to solutions, but match the search patterns.
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
                                var match = projectMatcher.Match(
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
                                        job.Info($"Read solution with {solution.Projects.Count} projects in it.");
                                        solutions.Add(solution);
                                    }
                                }
                            }
                        })
                );

            var solutionSet = solutions.ToHashSet();
            var orphanedProjectSet =
                projects.Except(solutions.SelectMany(sln => sln.Projects))
                    .ToHashSet();

            job.Info($"Discovered {solutionSet.Count} solutions and {orphanedProjectSet.Count} orphaned projects.");

            return
                (
                    Solutions: solutionSet,
                    OrphanedProjects: orphanedProjectSet
                );
        }
    }
}
