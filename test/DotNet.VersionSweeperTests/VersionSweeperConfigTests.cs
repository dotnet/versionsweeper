// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization.Metadata;
using Actions.Core;
using Actions.Core.Services;
using Actions.Core.Summaries;
using DotNet.VersionSweeper;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.VersionSweeperTests;

public class VersionSweeperConfigTests(ITestOutputHelper output)
{
    private const string TestConfigJson = """
        {
          "actionType": "pullRequest",
          "outOfSupportWithinDays": 90,
          "ignore": [
            "**/pinned-versions/**",
            "**/*.fsproj"
          ]
        }
        """;

    [Fact]
    public async Task ReadsConfigCorrectlyTest()
    {
        string fileName = "dotnet-versionsweeper.json";

        try
        {
            await File.WriteAllTextAsync(fileName, TestConfigJson);

            TestCoreService testJob = new(output);
            VersionSweeperConfig config = await VersionSweeperConfig.ReadAsync(".", testJob);

            Assert.NotNull(config);
            Assert.Equal(90, config.OutOfSupportWithinDays);
            Assert.Equal(ActionType.PullRequest, config.ActionType);
            Assert.Contains("**/pinned-versions/**", config.Ignore);
            Assert.Contains("**/*.fsproj", config.Ignore);
        }
        finally
        {
            File.Delete(fileName);
        }
    }
}

file class TestCoreService(ITestOutputHelper output) : ICoreService
{
    bool ICoreService.IsDebug { get; }
    Summary ICoreService.Summary { get; }

    ValueTask ICoreService.AddPathAsync(string inputPath) => throw new NotImplementedException();
    void ICoreService.WriteDebug(string message) => throw new NotImplementedException();
    void ICoreService.EndGroup() => throw new NotImplementedException();
    void ICoreService.WriteError(string message, AnnotationProperties? properties) => throw new NotImplementedException();
    ValueTask ICoreService.ExportVariableAsync(string name, string value) => throw new NotImplementedException();
    bool ICoreService.GetBoolInput(string name, InputOptions? options) => throw new NotImplementedException();
    string ICoreService.GetInput(string name, InputOptions? options) => throw new NotImplementedException();
    string[] ICoreService.GetMultilineInput(string name, InputOptions? options) => throw new NotImplementedException();
    string ICoreService.GetState(string name) => throw new NotImplementedException();
    ValueTask<T> ICoreService.GroupAsync<T>(string name, Func<ValueTask<T>> action) => throw new NotImplementedException();
    void ICoreService.WriteInfo(string message) => output.WriteLine(message);
    void ICoreService.WriteNotice(string message, AnnotationProperties? properties) => throw new NotImplementedException();
    ValueTask ICoreService.SaveStateAsync<T>(string name, T value, JsonTypeInfo<T> typeInfo) => throw new NotImplementedException();
    void ICoreService.SetCommandEcho(bool enabled) => throw new NotImplementedException();
    void ICoreService.SetFailed(string message) => throw new NotImplementedException();
    ValueTask ICoreService.SetOutputAsync<T>(string name, T value, JsonTypeInfo<T> typeInfo) => throw new NotImplementedException();
    void ICoreService.SetSecret(string secret) => throw new NotImplementedException();
    void ICoreService.StartGroup(string name) => throw new NotImplementedException();
    void ICoreService.WriteWarning(string message, AnnotationProperties? properties) => throw new NotImplementedException();
}
