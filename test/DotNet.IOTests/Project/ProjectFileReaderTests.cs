﻿using Xunit;
using System.Threading.Tasks;

namespace DotNet.IO.Tests
{
    public class ProjectFileReaderTests
    {
        [Fact]
        public async Task ReadProjectAsyncTest()
        {
            IProjectFileReader sut = new ProjectFileReader();
            var projectPath = "../../../DotNet.IOTests.csproj";

            var project = await sut.ReadProjectAsync(projectPath);
            Assert.Equal(4, project.TfmLineNumber);
            Assert.Single(project.Tfms);
            Assert.Equal("net5.0", project.Tfms[0]);
            Assert.Equal("Microsoft.NET.Sdk", project.Sdk);
        }
    }
}