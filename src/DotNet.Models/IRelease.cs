using System;

namespace DotNet.Models
{
    public interface IRelease
    {
        string TargetFrameworkMoniker { get; }
        SupportPhase SupportPhase { get; }
        DateTime? EndOfLifeDate { get; }
        string ReleaseNotesUrl { get; }
    }
}
