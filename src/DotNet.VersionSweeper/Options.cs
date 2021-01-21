using CommandLine;

namespace DotNet.VersionSweeper
{
    public class Options
    {
        [Option('o', "owner",
            HelpText = "The owner, for example: \"dotnet\".")]
        public string Owner { get; set; } = "dotnet";

        [Option('n', "name",
            HelpText = "The repository name, for example: \"samples\".")]
        public string Name { get; set; } = "samples";

        [Option('t', "token",
            Required = true,
            HelpText = "The GitHub personal-access token (PAT), or the token from GitHub action context.")]
        public string Token { get; set; } = null!;

        [Option('d', "dir",
            Default = ".",
            HelpText = "The root directory to start recursive searching from, defaults to: \".\".")]
        public string Directory { get; set; } = ".";

        [Option('p', "pattern",
            Default = "*.csproj",
            HelpText = "The search pattern to discover project files, defaults to: \"*.csproj\".")]
        public string SearchPattern { get; set; } = "*.csproj";
    }
}
