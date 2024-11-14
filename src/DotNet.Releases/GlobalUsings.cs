// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System.Collections.Concurrent;
global using System.Collections.ObjectModel;
global using System.Text.Json.Serialization;
global using System.Reflection;
global using DotNet.Extensions;
global using DotNet.Models;
global using DotNet.Releases;
global using DotNet.Releases.Extensions;
global using Microsoft.Deployment.DotNet.Releases;
global using Microsoft.Extensions.Caching.Memory;
global using Pathological.ProjectSystem.Models;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DotNet.GitHubTests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DotNet.ReleasesTests")]
