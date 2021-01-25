using DotNet.Models;
using Xunit;

namespace DotNet.ModelsTests
{
    public class FrameworkReleaseTests
    {
        [
            Theory,
            InlineData("3.5.0-sp1", "net35"),
            InlineData("4.0", "net40"),
            InlineData("4.5", "net45"),
            InlineData("4.5.1", "net451"),
            InlineData("4.5.2", "net452"),
            InlineData("4.6", "net46"),
            InlineData("4.8", "net48")
        ]
        public void FrameworkReleaseCorrectlyRepresentsTfm(
            string version, string expectedTfm) =>
            Assert.Equal(
                expectedTfm,
                new FrameworkRelease(version, null, null, null, null, null, null)
                    .TargetFrameworkMoniker);
    }
}
