using System;
using Xunit;
using static DotNet.VersionSweeper.EnvironmentVariableNames.GitHub;
using static DotNet.VersionSweeper.EnvironmentVariableNames.Sweeper;

namespace DotNet.VersionSweeper.Tests
{
    public class CommandLineExtensionsTests
    {
        [
            Theory,
            InlineData(Token, "-t", "some made up value!"),
            InlineData(Owner, "owner", "I own this..."),
            InlineData(Name, "n", "This is the repo name"),
            InlineData(Branch, "-b", "main"),
            InlineData(Directory, "dir", "root-dir"),
            InlineData(SearchPattern, "p", "*.fsproj"),
            InlineData("Invalid", "v", "this is not the value you're looking for!")
        ]
        public void OverrideFromEnvironmentVariablesTest(
            string envVar, string token, string expectedValue)
        {
            Environment.SetEnvironmentVariable(envVar, expectedValue);

            var args = new[] { token, "this is not the value you're looking for!" };
            Assert.Equal(expectedValue, args.OverrideFromEnvironmentVariables()[^1]);
        }
    }
}