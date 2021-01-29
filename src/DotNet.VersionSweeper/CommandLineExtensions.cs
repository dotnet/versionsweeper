using DotNet.GitHubActions;
using System;
using System.Collections.Generic;
using System.Linq;
using static DotNet.VersionSweeper.EnvironmentVariableNames.GitHub;
using static DotNet.VersionSweeper.EnvironmentVariableNames.Sweeper;

namespace DotNet.VersionSweeper
{
    public static class CommandLineExtensions
    {
        static readonly HashSet<string> _gitHubTokens = new() { "-t", "t", "token" };
        static readonly Dictionary<string, string[]> _environmentVariableNameToTokenMap =
            new()
            {
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

            // If the GitHub token was not passed in from the command-
            // line, trying appending it from the env vars.
            if (!args.Any(arg => _gitHubTokens.Contains(arg)))
            {
                var token = Environment.GetEnvironmentVariable(Token);
                if (token is { Length: > 0 })
                {
                    List<string> arguments = new(args)
                    {
                        "-t", token
                    };

                    return arguments.ToArray();
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
