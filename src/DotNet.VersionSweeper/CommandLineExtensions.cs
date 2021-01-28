using DotNet.GitHubActions;
using System;
using System.Collections.Generic;
using static DotNet.VersionSweeper.EnvironmentVariableNames.GitHub;
using static DotNet.VersionSweeper.EnvironmentVariableNames.Sweeper;

namespace DotNet.VersionSweeper
{
    public static class CommandLineExtensions
    {
        static readonly Dictionary<string, string[]> _environmentVariableNameToTokenMap =
            new()
            {
                [Token] = new[] { "-t", "t", "token" },
                [Branch] = new[] { "-b", "b", "branch" },
                [Directory] = new[] { "-d", "d", "dir" },
                [Name] = new[] { "-n", "n", "name" },
                [Owner] = new[] { "-o", "o", "owner" },
                [SearchPattern] = new[] { "-p", "p", "pattern" }
            };

        public static string[] OverrideFromEnvironmentVariables(
            this string[] args, IJobService jobService)
        {
            foreach (var (environmentVariable, tokens)
                in _environmentVariableNameToTokenMap)
            {
                if (TryFindReplacementValue(
                    environmentVariable,
                    args,
                    jobService,
                    out var index,
                    out var value,
                    tokens) &&
                    args.Length > index &&
                    value is not null)
                {
                    args[index] = value;
                }
            }

            return args;
        }

        static bool TryFindReplacementValue(
            string environmentVariableName,
            string[] args,
            IJobService jobService,
            out int index,
            out string? value,
            params string[] tokens)
        {
            value = jobService.GetInput(environmentVariableName);
            if (value is { Length: > 0})
            {
                foreach (var token in tokens)
                {
                    index = Array.IndexOf(args, token);
                    if (index > -1)
                    {
                        ++ index;
                        return true;
                    }
                }
            }

            index = -1;
            return false;
        }
    }
}
