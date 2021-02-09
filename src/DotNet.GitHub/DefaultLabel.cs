using Octokit;

namespace DotNet.GitHub
{
    class DefaultLabel
    {
        internal const string Name = "dotnet-target-version";
        internal const string Color = "512bd4";
        internal const string Description =
            "Issues and PRs automatically generated from the .NET version sweeper.";

        internal static NewLabel Value { get; } = new(Name, Color)
        {
            Description = Description
        };
    }
}
