using Xunit;
using System.Threading.Tasks;
using System.IO;

namespace DotNet.Releases.Solution.Tests
{
    public class SolutionFileReaderTests
    {
        [Fact]
        public async Task ReadSolutionAsyncTest()
        {
            ISolutionFileReader sut = new SolutionFileReader();
            var solutionPath = "../../../../../dotnet-versionsweeper.sln";

            var solution = await sut.ReadSolutionAsync(solutionPath);
            Assert.NotNull(solution);
            Assert.Equal(Path.GetFullPath(solutionPath), solution.Path);
            Assert.NotEmpty(solution.Projects);
        }
    }
}