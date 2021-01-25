using DotNet.Extensions;
using DotNet.Models;
using System.Linq;
using Xunit;

namespace DotNet.Releases.Tests
{
    public class CoreReleasesSerializationTests
    {
        const string TestReleaseIndexJson = @"{
    ""releases-index"": [
        {
            ""channel-version"": ""5.0"",
            ""latest-release"": ""5.0.2"",
            ""latest-release-date"": ""2021-01-12"",
            ""security"": true,
            ""latest-runtime"": ""5.0.2"",
            ""latest-sdk"": ""5.0.102"",
            ""product"": "".NET"",
            ""support-phase"": ""current"",
            ""releases.json"": ""https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/5.0/releases.json""
        },
        {
            ""channel-version"": ""1.0"",
            ""latest-release"": ""1.0.16"",
            ""latest-release-date"": ""2019-05-14"",
            ""security"": true,
            ""latest-runtime"": ""1.0.16"",
            ""latest-sdk"": ""1.1.14"",
            ""product"": "".NET Core"",
            ""support-phase"": ""eol"",
            ""eol-date"": ""2019-06-27"",
            ""releases.json"": ""https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/1.0/releases.json""
        }
    ]
}";

        [Fact]
        public void CoreReleasesDesializationTest()
        {
            var coreReleases = TestReleaseIndexJson.FromJson<CoreReleases>();
            var versionOne = coreReleases.ReleasesIndex.First(release => release.ChannelVersion == "1.0");
            var versionFive = coreReleases.ReleasesIndex.First(release => release.ChannelVersion == "5.0");

            static void AssertHasSameValues(
                ReleasesIndex expected, ReleasesIndex actual)
            {
                Assert.Equal(expected.ChannelVersion, actual.ChannelVersion);
                Assert.Equal(expected.LatestRelease, actual.LatestRelease);
                Assert.Equal(expected.LatestReleaseDate, actual.LatestReleaseDate);
                Assert.Equal(expected.Security, actual.Security);
                Assert.Equal(expected.LatestRuntime, actual.LatestRuntime);
                Assert.Equal(expected.LatestSdk, actual.LatestSdk);
                Assert.Equal(expected.Product, actual.Product);
                Assert.Equal(expected.SupportPhase, actual.SupportPhase);
                Assert.Equal(expected.ReleasesJson, actual.ReleasesJson);
            }

            var expectedVersionOne =
                new ReleasesIndex
                {
                    ChannelVersion = "1.0",
                    LatestRelease = "1.0.16",
                    LatestReleaseDate = "2019-05-14",
                    Security = true,
                    LatestRuntime = "1.0.16",
                    LatestSdk = "1.1.14",
                    Product = ".NET Core",
                    SupportPhase = SupportPhase.EndOfLife,
                    EolDate = "2019-06-27",
                    ReleasesJson = "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/1.0/releases.json"
                };

            var expectedVersionFive =
                new ReleasesIndex
                {
                    ChannelVersion = "5.0",
                    LatestRelease = "5.0.2",
                    LatestReleaseDate = "2021-01-12",
                    Security = true,
                    LatestRuntime = "5.0.2",
                    LatestSdk = "5.0.102",
                    Product = ".NET",
                    SupportPhase = SupportPhase.Current,
                    EolDate = "2019-06-27",
                    ReleasesJson = "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/5.0/releases.json"
                };

            AssertHasSameValues(expectedVersionOne, versionOne);
            AssertHasSameValues(expectedVersionFive, versionFive);
        }
    }
}