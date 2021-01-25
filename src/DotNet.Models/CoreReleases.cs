using DotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DotNet.Models
{
    public record CoreReleases(
        IEnumerable<ReleasesIndex> ReleasesIndex);

    public class ReleasesIndex : IRelease
    {
        public string ReleaseNotesUrl { get; set; } = null!;

        public string TargetFrameworkMoniker => Product switch
        {
            ".NET" => $"net{ChannelVersion}",
            ".NET Core" => $"netcoreapp{ChannelVersion}",

            _ => ChannelVersion
        };

        /// <summary>
        /// Do not call .AsSemanticVersion() on the channel version, it is not a SemVer.
        /// It only has "major.minor".
        /// </summary>
        public string ChannelVersion { get; init; } = null!;
        
        public string LatestRelease { get; init; } = null!;
        
        public string LatestReleaseDate { get; init; } = null!;

        [JsonPropertyName("releases.json")]
        public string ReleasesJson { get; init; } = null!;

        public SupportPhase SupportPhase { get; init; }

        /// <summary>
        /// When <see cref="SupportPhase.Current"/>, this is <see cref="null"/>.
        /// </summary>
        public string? EolDate { get; init; }

        public DateTime? EndOfLifeDate => EolDate.ToDateTime();

        /// <summary>
        /// A value indicating whether or not security patches are maintained.
        /// </summary>
        public bool Security { get; init; }
        
        public string LatestRuntime { get; init; } = null!;

        public string LatestSdk { get; init; } = null!;

        public string Product { get; init; } = null!;

        public string ToBrandString() => $"{Product} {ChannelVersion}";
    }
}
