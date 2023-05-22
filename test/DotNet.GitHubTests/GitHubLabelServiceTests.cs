// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.GitHub;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Octokit;
using Xunit;

namespace DotNet.GitHubTests;

public sealed class GitHubLabelServiceTests
{
    readonly ILogger<GitHubLabelService> _logger = new LoggerFactory().CreateLogger<GitHubLabelService>();
    readonly IMemoryCache _cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

    [Fact]
    public async Task GetOrCreateLabelAsyncCorrectlyReadsLabel()
    {
        IResilientGitHubClientFactory factory = SubstituteWith(
            new[] { new TestLabel("dotnet-target-version") },
            new("Label")).Factory;

        var sut = new GitHubLabelService(factory, _logger, _cache);
        Label label = await sut.GetOrCreateLabelAsync("unit", "test", "fake");

        Assert.Equal("dotnet-target-version", label.Name);
    }

    [Fact]
    public async Task GetOrCreateLabelAsyncCreatesLabel()
    {
        (IIssuesLabelsClient labelsClient, IIssuesClient _, IResilientGitHubClientFactory factory) = SubstituteWith(
            Array.Empty<TestLabel>(), new TestLabel("dotnet-target-version"));

        var sut = new GitHubLabelService(factory, _logger, _cache);
        Label label = await sut.GetOrCreateLabelAsync("unit", "test", "fake");

        await labelsClient.Received(1).Create(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<NewLabel>());

        Assert.Equal("dotnet-target-version", label.Name);
    }

    [Fact]
    public async Task GetOrCreateLabelAsyncOnlyCreatesLabelOnce()
    {
        (IIssuesLabelsClient labelsClient, IIssuesClient _, IResilientGitHubClientFactory factory) = SubstituteWith(
            Array.Empty<TestLabel>(), new TestLabel("dotnet-target-version"));

        int floodCount = 10_000;
        var sut = new GitHubLabelService(factory, _logger, _cache);
        Label[] labels = await Task.WhenAll(
            Enumerable.Range(0, floodCount)
                .Select(i => sut.GetOrCreateLabelAsync("unit", "test", "fake")
                .AsTask()));

        factory.Received(floodCount).Create(Arg.Any<string>());
        await labelsClient.Received(1).Create(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<NewLabel>());
        await labelsClient.Received(2).GetAllForRepository(
            Arg.Any<string>(), Arg.Any<string>());

        Assert.Equal("dotnet-target-version", labels[0].Name);
    }

    /// <summary>
    /// Creates several substitutions based on the given inputs
    /// </summary>
    /// <param name="allLabels">The labels returns from the <see cref="IIssuesLabelsClient.GetAllForRepository(string, string)"/> call.</param>
    /// <param name="createdLabel">The label returned from the <see cref="IIssuesLabelsClient.Create(string, string, NewLabel)"/> call</param>
    /// <returns></returns>
    private static (IIssuesLabelsClient LabelsClient, IIssuesClient IssuesClient, IResilientGitHubClientFactory Factory)
        SubstituteWith(
            IReadOnlyList<TestLabel> allLabels,
            TestLabel createdLabel)
    {
        IIssuesLabelsClient labelsClient = Substitute.For<IIssuesLabelsClient>();
        labelsClient.GetAllForRepository(null, null).ReturnsForAnyArgs(allLabels);
        labelsClient.Create(null, null, null).ReturnsForAnyArgs(createdLabel);

        IIssuesClient issuesClient = Substitute.For<IIssuesClient>();
        issuesClient.Labels.ReturnsForAnyArgs(labelsClient);

        IGitHubClient client = Substitute.For<IGitHubClient>();
        client.Issue.Returns(issuesClient);

        IResilientGitHubClientFactory factory = Substitute.For<IResilientGitHubClientFactory>();
        factory.Create(null).ReturnsForAnyArgs(client);

        return (labelsClient, issuesClient, factory);
    }
}

public sealed class TestLabel : Label
{
    public TestLabel(string name) :
        base(id: 7, url: "", name, nodeId: "", color: "", description: "test description", @default: false)
    {        
    }
}
