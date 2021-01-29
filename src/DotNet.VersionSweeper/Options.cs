using CommandLine;
using System;

namespace DotNet.VersionSweeper
{
    public class Options
    {
        string _repositoryName = null!;
        string _branchName = "refs/heads/main";

        [Option('o', "owner",
            HelpText = "The owner, for example: \"dotnet\". " +
            "Assign from `github.repository_owner`." +
            "Override with env var named `DOTNET_VERSIONSWEEPER_OWNER`.")]
        public string Owner { get; set; } = null!;

        [Option('n', "name",
            HelpText = "The repository name, for example: \"samples\". " +
            "Assign from `github.repository`. " +
            "Override with env var named `NAME`.")]
        public string Name
        {
            get => _repositoryName;
            set => ParseAndAssign(value, str => _repositoryName = str);
        }

        [Option('b', "branch",
            Default = "refs/heads/main",
            HelpText = "The branch name, for example: \"refs/heads/main\". " +
            "Assign from `github.ref`. " +
            "Override with env var named `BRANCH`.")]
        public string Branch
        {
            get => _branchName;
            set => ParseAndAssign(value, str => _branchName = str);
        }

        [Option('t', "token",
            Required = true,
            HelpText = "The GitHub personal-access token (PAT), or the token from GitHub action context. " +
            "Assign from `github.token`." +
            "Override with env var named `GITHUB_TOKEN`.`")]
        public string Token { get; set; } = null!;

        [Option('d', "dir",
            Default = ".",
            HelpText = "The root directory to start recursive searching from, defaults to: \".\"." +
            "Assign from `github.workspace`. " +
            "Override with env var named `DIRECTORY`.")]
        public string Directory { get; set; } = ".";

        [Option('p', "pattern",
            Default = "*.csproj",
            HelpText = "The search pattern to discover project files, defaults to: \"*.csproj\". " +
            "Override with env var named `PATTERN`.")]
        public string SearchPattern { get; set; } = "*.csproj";

        static void ParseAndAssign(string value, Action<string> assign)
        {
            if (value is { Length: > 0 } && assign is not null)
            {
                assign(value.Split("/")[^1]);
            }
        }
    }
}
