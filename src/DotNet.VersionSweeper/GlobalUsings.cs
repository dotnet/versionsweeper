// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Actions.Core.Extensions;
global using Actions.Core.Services;
global using CommandLine;
global using DotNet.Extensions;
global using DotNet.GitHub;
global using DotNet.Models;
global using DotNet.Releases;
global using DotNet.VersionSweeper;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Octokit;
global using Pathological.ProjectSystem.Models;
global using Pathological.ProjectSystem.Options;
global using Pathological.ProjectSystem.Services;
global using static System.Environment;
global using static CommandLine.Parser;
global using static DotNet.VersionSweeper.EnvironmentVariableNames;
global using ModelProject = Pathological.ProjectSystem.Models.Project;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DotNet.VersionSweeperTests")]
