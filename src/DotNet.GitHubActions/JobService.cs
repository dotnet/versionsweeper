// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using static System.Environment;

namespace DotNet.GitHubActions;

public sealed class JobService : IJobService
{
    /// <inheritdoc />
    public void AddPath(string inputPath)
    {
        IssueCommand(Commands.AddPath, message: inputPath);
        SetEnvironmentVariable(
            "PATH",
            $"{inputPath}{Path.PathSeparator}{GetEnvironmentVariable("PATH")}");
    }

    /// <inheritdoc />
    public bool IsDebug() => GetEnvironmentVariable("RUNNER_DEBUG") is "1";
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
    public string GetState(string name) => GetEnvironmentVariable($"STATE_{name}") ?? "";

    /// <inheritdoc />
    public void ExportVariable<T>(string name, T? value)
    {
        var convertedValue = value.ToCommandValue();
        SetEnvironmentVariable(name, convertedValue);

        var filePath = GetEnvironmentVariable("GITHUB_ENV") ?? "";
        if (filePath is { Length: > 0 })
        {
            var delimiter = "_GitHubActionsFileCommandDelimeter_";
            var commandValue = $"{name}<<{delimiter}{NewLine}{convertedValue}{NewLine}{delimiter}";
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
        var value = GetEnvironmentVariable(
            $"INPUT_{name.Replace(" ", "_").ToUpper()}") ?? "";

        return options?.Required ?? false && value is { Length: 0 }
            ? throw new($"Input required and not supplied: {name}")
            : value.Trim();
    }

    /// <inheritdoc />
    public void SaveState<T>(string stateName, T? stateValue)
    {
        if (stateValue is null) return;

        var gitHubStateFile = GetEnvironmentVariable("GITHUB_STATE");
        if (!string.IsNullOrWhiteSpace(gitHubStateFile))
        {
            using StreamWriter? textWriter = new(gitHubStateFile, true, Encoding.UTF8);
            textWriter.WriteLine($"{stateName}={stateValue}");
        }
    }

    /// <inheritdoc />
    public void SetFailed(string message, int? exitCode = null)
    {
        Error(message);
        Exit(exitCode ?? (int)ExitCode.Failure);
    }

    /// <inheritdoc />
    public void SetOutput(Dictionary<string, string>? properties = default)
    {
        if (properties is null) return;

        var gitHubOutputFile = GetEnvironmentVariable("GITHUB_OUTPUT");
        if (!string.IsNullOrWhiteSpace(gitHubOutputFile))
        {
            using StreamWriter? textWriter = new(gitHubOutputFile, true, Encoding.UTF8);
            foreach ((string key, string value) in properties)
            {
                textWriter.WriteLine($"{key}={value}");
            }
        }
    }

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
        var filePath = GetEnvironmentVariable($"GITHUB_{command}") ?? "";
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
            filePath, $"{message.ToCommandValue()}{NewLine}", Encoding.UTF8);
    }
}
