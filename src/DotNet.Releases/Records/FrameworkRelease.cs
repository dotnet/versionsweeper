using DotNet.Extensions;
using System.Collections.Generic;

namespace DotNet.Versions.Records
{
    public record FrameworkRelease(
        string Version,
        string Released,
        string ReleaseNotesUrl,
        string IncludedIn,
        string EndOfLife,
        Runtime Runtime,
        Developerpack DeveloperPack)
    {
        public string TargetFrameworkMoniker => $"v{Version}";

        public SupportPhase SupportPhase => EndOfLife.ToDateTime() switch
        {
            var date when date.Equals(default) && Version == "4.8" => SupportPhase.Current,
            var date when date.Equals(default) => SupportPhase.LongTermSupport,

            _ => SupportPhase.EndOfLife
        };
    }

    public record Runtime(
        string WebInstaller,
        string OfflineInstaller,
        Dictionary<string, string> LanguagePacks);

    public record Developerpack(string OfflineInstaller);
}
