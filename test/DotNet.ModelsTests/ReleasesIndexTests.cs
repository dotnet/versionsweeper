using DotNet.Models;
using Xunit;

namespace DotNet.ModelsTests
{
    public class ReleasesIndexTests
    {
        [
            Theory,
            InlineData("1.0", ".NET Core", "netcoreapp1.0"),
            InlineData("1.1", ".NET Core", "netcoreapp1.1"),
            InlineData("2.2", ".NET Core", "netcoreapp2.2"),
            InlineData("3.1", ".NET Core", "netcoreapp3.1"),
            InlineData("5.0", ".NET", "net5.0")
        ]
        public void ReleasesIndexCorrectlyRepresentsTfm(
            string version, string product, string expectedTfm) =>
            Assert.Equal(
                expectedTfm,
                new ReleaseIndex
                {
                    ChannelVersion = version,
                    Product = product
                }.TargetFrameworkMoniker);
    }
}
