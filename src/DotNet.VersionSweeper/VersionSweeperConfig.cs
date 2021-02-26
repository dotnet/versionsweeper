﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotNet.Extensions;
using DotNet.GitHubActions;

namespace DotNet.VersionSweeper
{
    /// <summary>
    /// Example 'dotnet-versionsweeper.json' file:
    /// {
    ///     "ignore": [
    ///         "**/pinned-versions/**",
    ///         "**/*.fsproj"
    ///     ]
    /// }
    /// </summary>
    public class VersionSweeperConfig
    {
        internal static string FileName = "dotnet-versionsweeper.json";

        [JsonPropertyName("ignore")]
        public string[] Ignore { get; init; } = Array.Empty<string>();

        internal static async Task<VersionSweeperConfig> ReadAsync(string root, IJobService job)
        {
            try
            {
                var fullPath = Path.Combine(root, FileName);
                if (File.Exists(fullPath))
                {
                    job.Info($"Reading '{fullPath}' config file.");

                    var configJson = await File.ReadAllTextAsync(fullPath);
                    var config = configJson.FromJson<VersionSweeperConfig>() ?? new();

                    job.Info($"Read {config.Ignore.Length} pattern(s) to ignore:");
                    job.Info($"{string.Join(",", config.Ignore.Select(val => $"\t{val}"))}");

                    return config;
                }
                else
                {
                    job.Info($"No '{fullPath}' config file to read.");
                }
            }
            catch
            {
                job.Warning($"Unable to read '{FileName}'.");
            }

            return new();
        }
    }
}
