using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DotNet.Versions
{
    public record Releases(
        IEnumerable<ReleasesIndex> ReleasesIndex);

    public class ReleasesIndex
    {
        public string ChannelVersion { get; init; }
        public string LatestRelease { get; init; }
        public string LatestReleaseDate { get; init; }
        public bool Security { get; init; }
        public string LatestRuntime { get; init; }
        public string LatestSdk { get; init; }
        public string Product { get; init; }
        public SupportPhase SupportPhase { get; init; }
        [JsonPropertyName("releases.json")]
        public string ReleasesJson { get; init; }
        public string? EolDate { get; init; }
    }
}
