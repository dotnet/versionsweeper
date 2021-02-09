using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DotNet.Releases.Tests
{
    public class FrameworkReleaseServiceTests
    {
        readonly IFrameworkReleaseIndexService _indexService = new FrameworkReleaseIndexService();
        readonly MemoryCache _cache = new(Options.Create(new MemoryCacheOptions()));

        [
            Theory,
            InlineData("4.5", "4.8"),
            InlineData("4.5.2", "4.8"),
            InlineData("4.6.1", "4.8"),
            InlineData("4.7", "4.8"),
            InlineData("4.7.2", "4.8"),
            InlineData("4.6", "4.8")
        ]
        public async Task GetNextLtsVersionAsyncTest(
            string releaseVersion, string expectedVersion)
        {
            IFrameworkReleaseService service =
                new FrameworkReleaseService(_indexService, _cache);

            var result = await service.GetNextLtsVersionAsync(releaseVersion);
            Assert.Equal(expectedVersion, result.Version);
        }
    }
}