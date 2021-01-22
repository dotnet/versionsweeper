using DotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Models
{
    public record FrameworkRelease(
        string Version,
        string Released,
        string ReleaseNotesUrl,
        string IncludedIn,
        string EndOfLife,
        FrameworkRuntime Runtime,
        Developerpack DeveloperPack) : IRelease
    {
        public string SemanticVersion => Version.Count(character => character == '.') switch
        {
            1 => $"{Version}.0",
            _ => Version
        };

        public string TargetFrameworkMoniker => $"v{Version}";

        public SupportPhase SupportPhase => EndOfLife.ToDateTime() switch
        {
            var date when date.Equals(default) && Version == "4.8" => SupportPhase.Current,
            var date when date.Equals(default) || date > DateTime.Now => SupportPhase.LongTermSupport,

            _ => SupportPhase.EndOfLife
        };

        public DateTime? EndOfLifeDate => EndOfLife.ToDateTime();
    }

    public record FrameworkRuntime(
        string WebInstaller,
        string OfflineInstaller,
        Dictionary<string, string> LanguagePacks);

    public record Developerpack(string OfflineInstaller);
}
