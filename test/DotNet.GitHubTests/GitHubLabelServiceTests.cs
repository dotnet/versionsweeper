using Xunit;
using DotNet.GitHub;
using System.Threading.Tasks;
using Octokit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;

namespace DotNet.GitHubTests
{
    public class GitHubLabelServiceTests
    {
        readonly ILogger<GitHubLabelService> _logger = new LoggerFactory().CreateLogger<GitHubLabelService>();
        readonly MemoryCache _cache = new(Options.Create(new MemoryCacheOptions()));

        [Fact]
        public async Task GetOrCreateLabelAsyncCorrectlyReadsLabel()
        {
            var labelsClient = Substitute.For<IIssuesLabelsClient>();
            labelsClient.GetAllForRepository(null, null).ReturnsForAnyArgs(
                new[]
                {
                    new TestLabel("dotnet-target-version")
                });
            var issuesClient = Substitute.For<IIssuesClient>();
            issuesClient.Labels.ReturnsForAnyArgs(labelsClient);
            var client = Substitute.For<IGitHubClient>();
            client.Issue.Returns(issuesClient);
            var factory = Substitute.For<IResilientGitHubClientFactory>();
            factory.Create(null).ReturnsForAnyArgs(client);

            var sut = new GitHubLabelService(factory, _logger, _cache);
            var label = await sut.GetOrCreateLabelAsync("unit", "test", "fake");

            Assert.Equal("dotnet-target-version", label.Name);
        }

        [Fact]
        public async Task GetOrCreateLabelAsyncCreatesLabel()
        {
            var labelsClient = Substitute.For<IIssuesLabelsClient>();
            labelsClient.GetAllForRepository(null, null).ReturnsForAnyArgs(
                Array.Empty<Label>());
            labelsClient.Create(null, null, null).ReturnsForAnyArgs(
                new TestLabel("dotnet-target-version"));
            var issuesClient = Substitute.For<IIssuesClient>();
            issuesClient.Labels.ReturnsForAnyArgs(labelsClient);
            var client = Substitute.For<IGitHubClient>();
            client.Issue.Returns(issuesClient);
            var factory = Substitute.For<IResilientGitHubClientFactory>();
            factory.Create(null).ReturnsForAnyArgs(client);

            var sut = new GitHubLabelService(factory, _logger, _cache);
            var label = await sut.GetOrCreateLabelAsync("unit", "test", "fake");

            await labelsClient.Received().Create(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<NewLabel>());

            Assert.Equal("dotnet-target-version", label.Name);
        }
    }

    public class TestLabel : Label
    {
        public TestLabel(string name) => Name = name;
    }
}