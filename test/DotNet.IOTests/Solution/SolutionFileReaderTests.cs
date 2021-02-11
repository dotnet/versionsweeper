using Xunit;
using System.Threading.Tasks;
using System.IO;

namespace DotNet.IO.Tests
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
            ISolutionFileReader sut = new SolutionFileReader(new ProjectFileReader());
            DirectoryInfo currentDirectory = new(Directory.GetCurrentDirectory());
            var fileName = "dotnet-versionsweeper.sln";
            var foundDirectory = TraverseToFile(currentDirectory, fileName);
            var solutionPath = Path.Combine(foundDirectory.FullName, fileName);

            var solution = await sut.ReadSolutionAsync(solutionPath);
            Assert.NotNull(solution);
            Assert.Equal(Path.GetFullPath(solutionPath), solution.FullPath);
            Assert.NotEmpty(solution.Projects);
        }

        public static DirectoryInfo TraverseToFile(DirectoryInfo directory, string filename)
        {
            try
            {
                while (directory.GetFiles(filename, SearchOption.TopDirectoryOnly).Length == 0)
                {
                    directory = directory.Parent;
                    if (directory == directory?.Root)
                    {
                        return null;
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }

            return directory;
        }
    }
}