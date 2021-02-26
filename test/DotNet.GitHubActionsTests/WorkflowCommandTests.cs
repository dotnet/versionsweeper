// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using DotNet.GitHubActions;
using Xunit;

namespace DotNet.GitHubActionsTests
{
    public class WorkflowCommandTests
    {
        public static IEnumerable<object[]> WorkflowCommandToStringInput = new[]
{
            new object[]
            {
                "some-name", 7, null, "::some-name::7"
            },
            new object[]
            {
                "another-name", true, null, "::another-name::true"
            },
            new object[]
            {
                "cmdr", false, new Dictionary<string, string> { ["k1"] = "v1" }, "::cmdr k1=v1::false"
            },
            new object[]
            {
                "~~~", "Hi friends!", null, "::~~~::Hi friends!"
            },
            new object[]
            {
                null, null, null, "::::"
            }
        };

        [
            Theory,
            MemberData(nameof(WorkflowCommandToStringInput))
        ]
        public void WorkflowCommandToStringTest<T>(
            string name, T message, Dictionary<string, string> properties, string expected)
        {
            WorkflowCommand<T> command = new(name, message, properties);
            Assert.Equal(expected, command.ToString());
        }
    }
}
