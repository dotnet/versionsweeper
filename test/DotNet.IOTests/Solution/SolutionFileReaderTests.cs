// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.IO;
using Xunit;

namespace DotNet.IOTests;

public class SolutionFileReaderTests
{
    [Fact]
    public async Task ReadSolutionAsyncTest()
    {
        var solutionPath = "test.sln";
        Dictionary<string, string> files = new()
        {
            ["sln-test.csproj"] = Constants.TestProjectXml,
            ["sln-test.json"] = Constants.TestProjectJson,
            [solutionPath] = Constants.TestSolutionXml
        };

        try
        {
            foreach (var (path, content) in files)
            {
                await File.WriteAllTextAsync(path, content);
            }

            ISolutionFileReader sut = new SolutionFileReader(new ProjectFileReader());

            var solution = await sut.ReadSolutionAsync(solutionPath);
            Assert.NotNull(solution);
            Assert.Equal(Path.GetFullPath(solutionPath), solution.FullPath);
            Assert.NotEmpty(solution.Projects);

            var project = solution.Projects.FirstOrDefault(p => !p.IsSdkStyle);
            Assert.Equal(18, project.TfmLineNumber);
            Assert.Single(project.Tfms);
            Assert.Equal("netcoreapp1.0", project.Tfms[0]);

            project = solution.Projects.FirstOrDefault(p => p.IsSdkStyle);
            Assert.Equal(4, project.TfmLineNumber);
            Assert.Single(project.Tfms);
            Assert.Equal("net5.0", project.Tfms[0]);
            Assert.Equal("Microsoft.NET.Sdk", project.Sdk);
        }
        finally
        {
            foreach (var path in files.Keys)
            {
                File.Delete(path);
            }
        }
    }
}
