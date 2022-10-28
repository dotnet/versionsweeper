// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System.Collections.Concurrent;
global using System.Net.Mime;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using DotNet.Extensions;
global using DotNet.Models;
global using Markdown;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Octokit;
global using Octokit.Internal;
global using ModelProject = DotNet.Models.Project;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DotNet.GitHubTests")]
