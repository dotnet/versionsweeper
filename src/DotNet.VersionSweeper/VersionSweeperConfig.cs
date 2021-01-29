using DotNet.Extensions;
using DotNet.GitHubActions;
using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

                    job.Info($"Read {config.Ignore.Length} patterns to ignore.");

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
