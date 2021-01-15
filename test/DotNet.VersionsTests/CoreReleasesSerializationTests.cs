using DotNet.Extensions;
using System.Linq;
using Xunit;

namespace DotNet.Versions.Tests
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

            Assert.Equal(
                new ReleasesIndex
                {
                    ChannelVersion = "1.0",
                    LatestRelease = "1.0.6",
                    LatestReleaseDate = "2019-05-14",
                    Security = true,
                    LatestRuntime = "1.0.16",
                    LatestSdk = "1.1.14",
                    Product = ".NET Core",
                    SupportPhase = SupportPhase.EndOfLife,
                    EolDate = "2019-06-27",
                    ReleasesJson = "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/1.0/releases.json"
                },
                versionOne);

            Assert.Equal(
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
                },
                versionOne);
        }
    }
}