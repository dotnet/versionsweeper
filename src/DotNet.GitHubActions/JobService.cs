// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNet.GitHubActions
{
    public class JobService : IJobService
    {
        /// <inheritdoc />
        public void AddPath(string inputPath)
        {
            IssueCommand(Commands.AddPath, message: inputPath);
            Environment.SetEnvironmentVariable(
                "PATH", $"{inputPath}{Path.PathSeparator}{Environment.GetEnvironmentVariable("PATH")}");
        }

        /// <inheritdoc />
        public bool IsDebug() => Environment.GetEnvironmentVariable("RUNNER_DEBUG") == "1";
        /// <inheritdoc />
        public void Info(string? message) => Console.WriteLine(message);
        /// <inheritdoc />
        public void Debug(string? message) => IssueCommand(Commands.Debug, message: message);
        /// <inheritdoc />
        public void Error(string? message) => IssueCommand(Commands.Error, message: message);
        /// <inheritdoc />
        public void Warning(string? message) => IssueCommand(Commands.Warning, message: message);
        /// <inheritdoc />
        public void SetSecret(string secret) => IssueCommand(Commands.AddMask, message: secret);
        /// <inheritdoc />
        public void StartGroup(string name) => IssueCommand(Commands.Group, message: name);
        /// <inheritdoc />
        public void EndGroup() => IssueCommand<object>(Commands.EndGroup);
        /// <inheritdoc />
        public void SetCommandEcho(bool enabled) => IssueCommand(Commands.Echo, message: enabled ? "on" : "off");
        /// <inheritdoc />
        public string GetState(string name) => Environment.GetEnvironmentVariable($"STATE_{name}") ?? "";

        /// <inheritdoc />
        public void ExportVariable<T>(string name, T? value)
        {
            var convertedValue = value.ToCommandValue();
            Environment.SetEnvironmentVariable(name, convertedValue);

            var filePath = Environment.GetEnvironmentVariable("GITHUB_ENV") ?? "";
            if (filePath is { Length: > 0 })
            {
                var delimiter = "_GitHubActionsFileCommandDelimeter_";
                var commandValue = $"{name}<<{delimiter}{Environment.NewLine}{convertedValue}{Environment.NewLine}{delimiter}";
                IssueFileCommand("ENV", commandValue);
            }
            else
            {
                IssueCommand(Commands.SetEnv, message: convertedValue);
            }
        }

        /// <inheritdoc />
        public string GetInput(string name, InputOptions? options = default)
        {
            var value = Environment.GetEnvironmentVariable(
                $"INPUT_{name.Replace(" ", "_").ToUpper()}") ?? "";

            return options?.Required ?? false && value is { Length: 0 }
                ? throw new($"Input required and not supplied: {name}")
                : value.Trim();
        }

        /// <inheritdoc />
        public void SaveState<T>(string stateName, T? stateValue) =>
            IssueCommand(Commands.SaveState, new() { ["name"] = stateName }, stateValue);

        /// <inheritdoc />
        public void SetFailed(string message, int? exitCode = null)
        {
            Error(message);
            Environment.Exit(exitCode ?? (int)ExitCode.Failure);
        }

        /// <inheritdoc />
        public void SetOutput(string? message, Dictionary<string, string>? properties = default) =>
            IssueCommand(Commands.SetOutput, properties, message);

        static void IssueCommand<T>(
            string commandName,
            Dictionary<string, string>? properties = default,
            T? message = default)
        {
            WorkflowCommand<T> workflowCommand = new(commandName, message, properties);
            Console.WriteLine(workflowCommand.ToString());
        }

        static void IssueFileCommand(
            string command,
            object? message)
        {
            var filePath = Environment.GetEnvironmentVariable($"GITHUB_{command}") ?? "";
            if (filePath is { Length: > 0 })
            {
                throw new(
                    $"Unable to find environment variable for file command {command}");
            }

            if (!File.Exists(filePath))
            {
                throw new($"Missing file at path: {filePath}");
            }

            File.AppendAllText(
                filePath, $"{message.ToCommandValue()}{Environment.NewLine}", Encoding.UTF8);
        }
    }
}
