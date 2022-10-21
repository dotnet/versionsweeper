// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.Models;
using Microsoft.Deployment.DotNet.Releases;
using Xunit;

namespace DotNet.ModelsTests;

public sealed class ReleaseFactoryTests
{
    [Fact]
    public void CreateTest()
    {
        var example = new Example();
        var release = ReleaseFactory.Create(
            example,
            _ => "Does this thing work",
            "test",
            SupportPhase.LTS,
            null,
            "some-path.md");

        Assert.Equal(SupportPhase.LTS, release.SupportPhase);
        Assert.Equal("Does this thing work", release.ToBrandString());
        Assert.Equal("test", release.TargetFrameworkMoniker);
        Assert.Null(release.EndOfLifeDate);
        Assert.Equal("some-path.md", release.ReleaseNotesUrl);
    }

    class Example
    {
    }
}
