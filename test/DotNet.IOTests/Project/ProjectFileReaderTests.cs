// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;
using DotNet.IO;
using Xunit;

namespace DotNet.IOTests
{
    public class ProjectFileReaderTests
    {
        [Fact]
        public async Task ReadProjectAndParseXmlCorrectly()
        {
            var projectPath = "test.csproj";

            try
            {
                await File.WriteAllTextAsync(projectPath, Constants.TestProjectXml);

                IProjectFileReader sut = new ProjectFileReader();

                var project = await sut.ReadProjectAsync(projectPath);
                Assert.Equal(4, project.TfmLineNumber);
                Assert.Single(project.Tfms);
                Assert.Equal("net5.0", project.Tfms[0]);
                Assert.Equal("Microsoft.NET.Sdk", project.Sdk);
            }
            finally
            {
                File.Delete(projectPath);
            }
        }

        [
            Theory,
            InlineData("test-1.json", Constants.TestProjectJson, 18, "netcoreapp1.0"),
            InlineData("test-2.json", Constants.TestProjectJsonMultipleTfms, 27, "dnx46", "dnxcore50")
        ]
        public async Task ReadProjectAndParseJsonCorrectly(
            string projectPath, string content, int expectedLineNumber, params string[] expectedTfms)
        {

            try
            {
                await File.WriteAllTextAsync(projectPath, content);

                IProjectFileReader sut = new ProjectFileReader();

                var project = await sut.ReadProjectAsync(projectPath);
                Assert.Equal(expectedLineNumber, project.TfmLineNumber);
                Assert.Equal(expectedTfms.Length, project.Tfms.Length);
                for (var i = 0; i < expectedTfms.Length; ++i)
                {
                    Assert.Equal(expectedTfms[i], project.Tfms[i]);
                }
                Assert.Null(project.Sdk);
            }
            finally
            {
                File.Delete(projectPath);
            }
        }
    }
}
