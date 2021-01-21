using System.Collections.Generic;

namespace DotNet.Models
{
    public record CoreReleaseDetails(
        string ChannelVersion, // Do not call .AsSemanticVersion() on this, "major.minor".
        string LatestRelease,
        string LatestReleaseDate,
        string LatestRuntime,
        string LatestSdk,
        SupportPhase SupportPhase,
        string LifecyclePolicy,
        IEnumerable<Release> Releases,
        Intellisense Intellisense);

    public record Intellisense(
        string Version,
        string VersionDisplay,
        IEnumerable<File> Files);

    public record File(
        string Lang,
        string Name,
        string Rid,
        string Url,
        string Hash,
        string Akams);

    public record Release(
        string ReleaseDate,
        string ReleaseVersion,
        bool Security,
        IEnumerable<CveList> CveList,
        string ReleaseNotes,
        Runtime Runtime,
        Sdk Sdk,
        IEnumerable<Sdk> Sdks,
        AspnetcoreRuntime AspnetcoreRuntime,
        WindowsDesktop Windowsdesktop,
        Symbols Symbols);

    public record Runtime(
        string Version,
        string VersionDisplay,
        string VsVersion,
        string VsMacVersion,
        IEnumerable<File> Files);

    public record Sdk(
        string Version,
        string VersionDisplay,
        string RuntimeVersion,
        string VsVersion,
        string VsMacVersion,
        string VsSupport,
        string VsMacSupport,
        string CsharpVersion,
        string FsharpVersion,
        string VbVersion,
        IEnumerable<File> Files);

    public record AspnetcoreRuntime(
        string Version,
        string VersionDisplay,
        IEnumerable<string> VersionAspnetcoremodule,
        string VsVersion,
        IEnumerable<File> Files);

    public record WindowsDesktop(
        string Version,
        string VersionDisplay,
        IEnumerable<File> Files);

    public record Symbols(
        string Version,
        IEnumerable<File> Files);

    public record CveList(
        string CveId,
        string CveUrl);
}