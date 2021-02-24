using Xunit;
using Microsoft.Deployment.DotNet.Releases;
using DotNet.Models;

namespace DotNet.ModelsTests
{
    public class ReleaseFactoryTests
    {
        [Fact]
        public void CreateTest()
        {
            var example = new Example();
            var release = ReleaseFactory.Create(
                example,
                _ => "Does this thing work",
                "test",
                SupportPhase.LTS,
                null,
                "some-path.md");

            Assert.Equal(SupportPhase.LTS, release.SupportPhase);
            Assert.Equal("Does this thing work", release.ToBrandString());
            Assert.Equal("test", release.TargetFrameworkMoniker);
            Assert.Null(release.EndOfLifeDate);
            Assert.Equal("some-path.md", release.ReleaseNotesUrl);
        }

        class Example
        {
        }
    }
}