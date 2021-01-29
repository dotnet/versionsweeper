using CommandLine;
using System;
using static System.Environment;
using static DotNet.VersionSweeper.EnvironmentVariableNames;

namespace DotNet.VersionSweeper
{
    public class Options
    {
        string _repositoryName = null!;
        string _branchName = null!;

        public Options()
        {
            Owner = GetEnvironmentVariable(Sweeper.Owner);
            Name = GetEnvironmentVariable(Sweeper.Name);
            Branch = GetEnvironmentVariable(Sweeper.Branch);
            Directory = GetEnvironmentVariable(Sweeper.Directory);
            SearchPattern = GetEnvironmentVariable(Sweeper.SearchPattern);
            Token = GetEnvironmentVariable(EnvironmentVariableNames.GitHub.Token);
        }

        [Option('o', "owner",
            Required = true,
            HelpText = "The owner, for example: \"dotnet\". " +
            "Assign from `github.repository_owner`." +
            "Override with env var named `OWNER`.")]
        public string? Owner { get; set; } = null!;

        [Option('n', "name",
            Required = true,
            HelpText = "The repository name, for example: \"samples\". " +
            "Assign from `github.repository`. " +
            "Override with env var named `NAME`.")]
        public string? Name
        {
            get => _repositoryName;
            set => ParseAndAssign(value, str => _repositoryName = str);
        }

        [Option('b', "branch",
            Required = true,
            HelpText = "The branch name, for example: \"refs/heads/main\". " +
            "Assign from `github.ref`. " +
            "Override with env var named `BRANCH`.")]
        public string? Branch
        {
            get => _branchName;
            set => ParseAndAssign(value, str => _branchName = str);
        }

        [Option('t', "token",
            Required = true,
            HelpText = "The GitHub personal-access token (PAT), or the token from GitHub action context. " +
            "Assign from `github.token`." +
            "Override with env var named `GITHUB_TOKEN`.`")]
        public string? Token { get; set; } = null!;

        [Option('d', "dir",
            Required = true,
            HelpText = "The root directory to start recursive searching from." +
            "Assign from `github.workspace`. " +
            "Override with env var named `DIRECTORY`.")]
        public string? Directory { get; set; } = null!;

        [Option('p', "pattern",
            Required = true,
            HelpText = "The search pattern to discover project files. " +
            "Override with env var named `PATTERN`.")]
        public string? SearchPattern { get; set; } = null!;

        static void ParseAndAssign(string? value, Action<string> assign)
        {
            if (value is { Length: > 0 } && assign is not null)
            {
                assign(value.Split("/")[^1]);
            }
        }
    }
}
