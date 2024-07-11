// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.VersionSweeper;

/// <summary>
/// Example <c>dotnet-versionsweeper.json</c> file:
/// <code>
/// {
///   "actionType": "pullRequest",
///   "outOfSupportWithinDays": 90,
///   "ignore": [
///     "**/pinned-versions/**",
///     "**/*.fsproj"
///   ]
/// }
/// </code>
/// </summary>
public sealed class VersionSweeperConfig
{
    private static VersionSweeperConfig? s_cachedConfig = null;

    internal const string FileName = "dotnet-versionsweeper.json";

    [JsonPropertyName("ignore")]
    public string[] Ignore { get; init; } = [];

    [JsonPropertyName("actionType")]
    public ActionType ActionType { get; init; } = ActionType.CreateIssue;

    [JsonPropertyName("outOfSupportWithinDays")]
    public int OutOfSupportWithinDays { get; init; } = 90;

    /// <summary>
    /// Returns the read configuration, or a default configuration if the file is not found.
    /// The configuration is cached for the lifetime of the process.
    /// </summary>
    internal static async Task<VersionSweeperConfig> ReadAsync(string root, ICoreService job)
    {
        if (s_cachedConfig is not null)
        {
            return s_cachedConfig;
        }

        try
        {
            string fullPath = Path.Combine(root, FileName);
            if (File.Exists(fullPath))
            {
                job.WriteInfo($"Reading '{fullPath}' config file.");

                string configJson = await File.ReadAllTextAsync(fullPath);
                VersionSweeperConfig config = configJson.FromJson<VersionSweeperConfig>() ?? new();

                job.WriteInfo($"Read {config.Ignore.Length} pattern(s) to ignore:");
                job.WriteInfo($"{string.Join(",", config.Ignore.Select(val => $"\t{val}"))}");
                job.WriteInfo($"Intended version sweeper type: {config.ActionType}");
                job.WriteInfo($"Out of support within days: {config.OutOfSupportWithinDays}");

                s_cachedConfig = config;

                return config;
            }
            else
            {
                job.WriteInfo($"No '{fullPath}' config file to read.");
            }
        }
        catch
        {
            job.WriteWarning($"Unable to read '{FileName}'.");
        }

        return new();
    }
}
