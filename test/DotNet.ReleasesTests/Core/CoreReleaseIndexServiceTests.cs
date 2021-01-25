using Xunit;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DotNet.Releases.Tests
{
    public class CoreReleaseIndexServiceTests : IDisposable
    {
        readonly HttpClient _client = new();
        readonly MemoryCache _cache = new(Options.Create(new MemoryCacheOptions()));

        [
            Theory,
            InlineData("1.1", "2.1"),
            InlineData("1.0", "2.1"),
            InlineData("2.2.8", "3.1"),
            InlineData("3.0.3", "3.1"),
            InlineData("3.1.11", "5.0")
        ]
        public async Task GetNextLtsVersionAsyncTest(
            string releaseVersion, string expectedVersion)
        {
            ICoreReleaseIndexService service = new CoreReleaseIndexService(_client, _cache);

            var result = await service.GetNextLtsVersionAsync(releaseVersion);
            Assert.Equal(expectedVersion, result.ChannelVersion);
        }

        void IDisposable.Dispose() => _client?.Dispose();
    }
}