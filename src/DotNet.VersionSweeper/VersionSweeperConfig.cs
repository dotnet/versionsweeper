// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.VersionSweeper;

/// <summary>
/// Example 'dotnet-versionsweeper.json' file:
/// {
///     "type": "pullRequest",
///     "outOfSupportWithinDays": 90,
///     "ignore": [
///         "**/pinned-versions/**",
///         "**/*.fsproj"
///     ]
/// }
/// </summary>
public class VersionSweeperConfig
{
    internal const string FileName = "dotnet-versionsweeper.json";

    [JsonPropertyName("ignore")]
    public string[] Ignore { get; init; } = Array.Empty<string>();

    [JsonPropertyName("type")]
    public ActionType Type { get; init; } = ActionType.CreateIssue;

    [JsonPropertyName("outOfSupportWithinDays")]
    public int OutOfSupportWithinDays { get; init; } = 0;

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
                job.Info($"Intended version sweeper type: {config.Type}");
                job.Info($"Out of support within days: {config.OutOfSupportWithinDays}");

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
