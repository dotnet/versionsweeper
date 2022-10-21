// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.IO;
using Xunit;

namespace DotNet.IOTests;

public class DockerfileReaderTests
{
    [Fact]
    public async Task ReadDcokerfileAndParsesCorrectly()
    {
        var dockerfilePath = "Dockerfile";

        try
        {
            await File.WriteAllTextAsync(dockerfilePath, Constants.DockerfileWithMultipleTfms);

            IDockerfileReader sut = new DockerfileReader();

            var dockerfile = await sut.ReadDockerfileAsync(dockerfilePath);
            Assert.Equal(2, dockerfile.ImageDetails.Count);
        }
        finally
        {
            File.Delete(dockerfilePath);
        }
    }
}
