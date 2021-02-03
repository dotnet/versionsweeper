using Xunit;
using System.Threading.Tasks;

namespace DotNet.Releases.Tests
{
    public class ProjectFileReaderTests
    {
        [Fact]
        public async Task ReadProjectTfmsAsyncTest()
        {
            IProjectFileReader sut = new ProjectFileReader();
            var projectPath = "../../../DotNet.ReleasesTests.csproj";

            var (lineNumber, tfms) = await sut.ReadProjectTfmsAsync(projectPath);
            Assert.Equal(4, lineNumber);
            Assert.Single(tfms);
            Assert.Equal("net5.0", tfms[0]);
        }
    }
}