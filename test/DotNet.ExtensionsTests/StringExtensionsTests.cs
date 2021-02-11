using System.IO;
using Xunit;

namespace DotNet.Extensions.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void FirstAndLastSegementsOfPathTest()
        {
            var d = Path.DirectorySeparatorChar;
            foreach (var (input, expected)
                in new[]
                {
                    (null, null),
                    ($"path-project.csproj", "path-project.csproj"),
                    ($"example{d}of{d}some{d}path{d}project.csproj", "example/.../project.csproj")
                })
            {
                Assert.Equal(expected, input.FirstAndLastSegmentOfPath("..."));
            }
        }
    }
}