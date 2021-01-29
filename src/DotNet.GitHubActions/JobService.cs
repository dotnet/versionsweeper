using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNet.GitHubActions
{
    public class JobService : IJobService
    {
        public void AddPath(string inputPath)
        {
            IssueCommand(Commands.AddPath, message: inputPath);
            Environment.SetEnvironmentVariable(
                "PATH", $"{inputPath}{Path.PathSeparator}{Environment.GetEnvironmentVariable("PATH")}");
        }

        public void Debug(string message) => IssueCommand(Commands.Debug, message: message);

        public void EndGroup() => IssueCommand(Commands.EndGroup);

        public void Error(string message) => IssueCommand(Commands.Error, message: message);

        public void ExportVariable(string name, object? value)
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

        public string GetInput(string name, InputOptions? options = default)
        {
            var value = Environment.GetEnvironmentVariable(
                $"INPUT_{name.Replace(" ", "_").ToUpper()}") ?? "";

            return options?.Required ?? false && value is { Length: 0 }
                ? throw new($"Input required and not supplied: {name}")
                : value.Trim();
        }

        public string GetState(string name) =>
            Environment.GetEnvironmentVariable($"STATE_{name}") ?? "";

        public void Info(string message) => Console.WriteLine(message);

        public bool IsDebug() => Environment.GetEnvironmentVariable("RUNNER_DEBUG") == "1";

        public void SaveState(string name, object? value) =>
            IssueCommand(Commands.SaveState, new Dictionary<string, string>() { [nameof(name)] = name }, value);

        public void SetCommandEcho(bool enabled) =>
            IssueCommand(Commands.Echo, message: enabled ? "on" : "off");

        public void SetFailed(string message)
        {
            Error(message);

            Environment.Exit((int)ExitCode.Failure);
        }

        public void SetOutput(string name, object? value) =>
            IssueCommand(Commands.SetOutput, new Dictionary<string, string>() { [nameof(name)] = name }, value);

        public void SetSecret(string secret) =>
            IssueCommand(Commands.AddMask, message: secret);

        public void StartGroup(string name) =>
            IssueCommand(Commands.Group, message: name);

        public void Warning(string message) =>
            IssueCommand(Commands.Warning, message: message);

        static void IssueCommand(
            string command,
            IDictionary<string, string>? commandProperties = default,
            object? message = default)
        {
            WorkflowCommand workflowCommand = new(command, message, commandProperties);
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
