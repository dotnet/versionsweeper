using Xunit;
using System.Threading.Tasks;
using System.IO;

namespace DotNet.IO.Tests
{
    public class SolutionFileReaderTests
    {
        [Fact]
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