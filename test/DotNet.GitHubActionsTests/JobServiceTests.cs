// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Cryptography.X509Certificates;
using DotNet.GitHubActions;
using Xunit;

namespace DotNet.GitHubActionsTests;

public sealed class JobServiceTests : IDisposable
{
    static readonly TextWriter s_consoleOut = Console.Out;

    public static IEnumerable<object[]> WriteSaveStateInput = new[]
    {
            new object[]
            {
                "test-state",
                2,
                $"test-state=2{Environment.NewLine}"
            },
            new object[]
            {
                "pickle",
                "chips",
                $"pickle=chips{Environment.NewLine}"
            }
        };

    [
        Theory,
        MemberData(nameof(WriteSaveStateInput))
    ]
    public void WriteSaveStateCommandTest<T>(
        string stateName,
        T stateValue,
        string expected)
    {
        var tempFile = Path.GetTempFileName();
        Environment.SetEnvironmentVariable("GITHUB_STATE", tempFile);
        IJobService sut = new JobService();

        sut.SaveState(stateName, stateValue);

        var stateFile = Environment.GetEnvironmentVariable("GITHUB_STATE");
        var actual = File.ReadAllText(stateFile);
        try
        {
            Assert.Equal(expected, actual);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    public static IEnumerable<object[]> WriteSetOutputInput = new[]
    {
            new object[]
            {
                new Dictionary<string, string>
                {
                    ["name"] = "summary"
                },
                $"name=summary{Environment.NewLine}"
            },
            new object[]
            {
                null,
                ""
            },
            new object[]
            {
                new Dictionary<string, string>
                {
                    ["prop1"] = "Value 1",
                    ["prop2"] = "Value 2"
                },
                $"prop1=Value 1{Environment.NewLine}prop2=Value 2{Environment.NewLine}"
            }
        };

    [
        Theory,
        MemberData(nameof(WriteSetOutputInput))
    ]
    public void WriteSetOutputCommandTest(
        Dictionary<string, string> properties,
        string expected)
    {
        var tempFile = Path.GetTempFileName();
        Environment.SetEnvironmentVariable("GITHUB_OUTPUT", tempFile);
        IJobService sut = new JobService();

        sut.SetOutput(properties);

        var outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
        var actual = File.ReadAllText(outputFile);
        try
        {
            Assert.Equal(expected, actual);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void WriteWarningMessageTest()
    {
        var interceptor = InterceptOut();
        IJobService sut = new JobService();
        sut.Warning("Um, not sure....but something feels off!");

        AssertCommand(
            interceptor,
            new[] { "::warning::Um, not sure....but something feels off!" });
    }

    [Fact]
    public void WriteDebugMessageTest()
    {
        var interceptor = InterceptOut();
        IJobService sut = new JobService();
        sut.Debug("Does this really work?");

        AssertCommand(
            interceptor,
            new[] { "::debug::Does this really work?" });
    }

    [Fact]
    public void GroupMessagesTest()
    {
        var interceptor = InterceptOut();
        IJobService sut = new JobService();
        sut.StartGroup("example");
        sut.Info("Testing... 1, 2, free?!");
        sut.EndGroup();

        AssertCommand(
            interceptor,
            new[] { "::group::example", "Testing... 1, 2, free?!", "::endgroup::" });
    }

    static StringWriter InterceptOut()
    {
        StringWriter interceptor = new();
        Console.SetOut(interceptor);

        return interceptor;
    }

    static void AssertCommand(StringWriter writer, string[] expectedCommands) =>
        AssertCommand(writer.ToString(), expectedCommands);

    static void AssertCommand(string actualRaw, string[] expectedCommands)
    {
        var actualCommands =
            actualRaw.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(expectedCommands.Length, actualCommands.Length);

        for (var i = 0; i < expectedCommands.Length; ++i)
        {
            Assert.Equal(expectedCommands[i], actualCommands[i]);
        }
    }

    public void Dispose() => Console.SetOut(s_consoleOut);
}
