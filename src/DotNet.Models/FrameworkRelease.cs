using DotNet.Extensions;
using System;
using System.Collections.Generic;

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
        public string TargetFrameworkMoniker => Version switch
            {
                "3.5.0-sp1" => "net35",
                _ => $"net{Version.Replace(".", "")}"
            };

        public SupportPhase SupportPhase => EndOfLifeDate switch
        {
            var date when date is null && Version == "4.8" => SupportPhase.Current,
            var date when date is null || date > DateTime.Now => SupportPhase.LongTermSupport,

            _ => SupportPhase.EndOfLife
        };

        public DateTime? EndOfLifeDate => EndOfLife.ToDateTime();

        public string ToBrandString() => $".NET Framework {TargetFrameworkMoniker}";
    }

    public record FrameworkRuntime(
        string WebInstaller,
        string OfflineInstaller,
        Dictionary<string, string> LanguagePacks);

    public record Developerpack(string OfflineInstaller);
}
