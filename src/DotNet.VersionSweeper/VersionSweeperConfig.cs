using DotNet.Extensions;
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

        internal static async Task<VersionSweeperConfig> ReadAsync(string root)
        {
            try
            {
                var fullPath = Path.Combine(root, FileName);
                if (File.Exists(fullPath))
                {
                    var configJson = await File.ReadAllTextAsync(fullPath);
                    return configJson.FromJson<VersionSweeperConfig>() ?? new();
                }
            }
            catch
            {
                Console.Error.WriteLine($"Unable to read '{FileName}'.");
            }

            return new();
        }
    }
}
