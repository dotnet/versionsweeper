// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System.Collections.Concurrent;
global using System.Collections.Immutable;
global using System.Text;
global using System.Text.Json.Serialization;
global using CommandLine;
global using DotNet.Extensions;
global using DotNet.GitHub;
global using Actions.Core;
global using Actions.Core.Services;
global using Actions.Core.Extensions;
global using DotNet.IO;
global using DotNet.Models;
global using DotNet.Releases;
global using DotNet.VersionSweeper;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.FileSystemGlobbing;
global using Microsoft.Extensions.Hosting;
global using Octokit;
global using static System.Environment;
global using static CommandLine.Parser;
global using static DotNet.VersionSweeper.EnvironmentVariableNames;
global using ModelProject = DotNet.Models.Project;
