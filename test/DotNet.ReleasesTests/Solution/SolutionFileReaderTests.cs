using Xunit;
using System.Threading.Tasks;
using System.IO;

namespace DotNet.Releases.Solution.Tests
{
    public class SolutionFileReaderTests
    {
#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Fact(Skip =
            "This is skipped on the build server, but will run locally." +
            "The GitHub action has issues finding the .sln path.")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
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