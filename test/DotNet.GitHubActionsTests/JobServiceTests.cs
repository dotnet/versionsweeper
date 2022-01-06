// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
                new[]
                {
                    $"::{Commands.SaveState} name=test-state::2"
                }
            },
            new object[]
            {
                "pickle",
                "chips",
                new[]
                {
                    $"::{Commands.SaveState} name=pickle::chips"
                }
            }
        };

    [
        Theory,
        MemberData(nameof(WriteSaveStateInput))
    ]
    public void WriteSaveStateCommandTest<T>(
        string stateName,
        T stateValue,
        string[] expectedCommands)
    {
        var interceptor = InterceptOut();
        IJobService sut = new JobService();

        sut.SaveState(stateName, stateValue);

        AssertCommand(
            interceptor,
            expectedCommands);
    }

    public static IEnumerable<object[]> WriteSetOutputInput = new[]
    {
            new object[]
            {
                new Dictionary<string, string>
                {
                    ["name"] = "summary"
                },
                "Everything worked as expected",
                new[]
                {
                    $"::{Commands.SetOutput} name=summary::Everything worked as expected"
                }
            },
            new object[]
            {
                null,
                "deftones",
                new[]
                {
                    $"::{Commands.SetOutput}::deftones"
                }
            },
            new object[]
            {
                null,
                "percent % percent % cr \r cr \r lf \n lf \n",
                new[]
                {
                    $"::{Commands.SetOutput}::percent %25 percent %25 cr %0D cr %0D lf %0A lf %0A"
                }
            },
            new object[]
            {
                null,
                "%25 %25 %0D %0D %0A %0A %3A %3A %2C %2C",
                new[]
                {
                    $"::{Commands.SetOutput}::%2525 %2525 %250D %250D %250A %250A %253A %253A %252C %252C"
                }
            },
            new object[]
            {
                new Dictionary<string, string>
                {
                    ["prop1"] = "Value 1",
                    ["prop2"] = "Value 2"
                },
                "example",
                new[]
                {
                    $"::{Commands.SetOutput} prop1=Value 1, prop2=Value 2::example"
                }
            }
        };

    [
        Theory,
        MemberData(nameof(WriteSetOutputInput))
    ]
    public void WriteSetOutputCommandTest(
        Dictionary<string, string> properties,
        string message,
        string[] expectedCommands)
    {
        var interceptor = InterceptOut();
        IJobService sut = new JobService();

        sut.SetOutput(message, properties);

        AssertCommand(
            interceptor,
            expectedCommands);
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

    static void AssertCommand(StringWriter writer, string[] expectedCommands)
    {
        var actualCommands =
            writer.ToString()
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(expectedCommands.Length, actualCommands.Length);

        for (var i = 0; i < expectedCommands.Length; ++i)
        {
            Assert.Equal(expectedCommands[i], actualCommands[i]);
        }
    }

    public void Dispose() => Console.SetOut(s_consoleOut);
}
