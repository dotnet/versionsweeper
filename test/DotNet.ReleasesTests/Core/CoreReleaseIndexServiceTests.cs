using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using DotNet.Extensions;
using Microsoft.Deployment.DotNet.Releases;
using DotNet.Releases.Extensions;
using System.Text.Json;

namespace DotNet.Releases.Tests
{
    public class CoreReleaseIndexServiceTests
    {
        readonly MemoryCache _cache = new(Options.Create(new MemoryCacheOptions()));

        [
            Theory,
            InlineData("1.1.0", "2.1"),
            InlineData("1.0.0", "2.1"),
            InlineData("2.2.8", "3.1"),
            InlineData("3.0.3", "3.1"),
            InlineData("3.1.11", "3.1")
        ]
        public async Task GetNextLtsVersionAsyncTest(
            string releaseVersion, string expectedVersion)
        {
            ICoreReleaseIndexService service = new CoreReleaseIndexService(_cache);

            var result = await service.GetNextLtsVersionAsync(releaseVersion);
            Assert.Equal(expectedVersion, result.ProductVersion);
        }

        [
            Theory,
            InlineData("1.0", ".NET Core", "netcoreapp1.0"),
            InlineData("1.1", ".NET Core", "netcoreapp1.1"),
            InlineData("2.2", ".NET Core", "netcoreapp2.2"),
            InlineData("3.1", ".NET Core", "netcoreapp3.1"),
            InlineData("5.0", ".NET", "net5.0")
        ]
        public void ReleasesIndexCorrectlyRepresentsTfm(
            string version, string productName, string expectedTfm)
        {
            var product = "{}".FromJson<Product>();
            static void WorkAroundDeserializationLimitation<T>(Product product, string propName, T propValue)
            {
                typeof(Product).GetProperty(propName).SetValue(product, propValue, null);
            }

            WorkAroundDeserializationLimitation(product, nameof(Product.ProductVersion), version);
            WorkAroundDeserializationLimitation(product, nameof(Product.ProductName), productName);

            Assert.Equal(expectedTfm, product.GetTargetFrameworkMoniker());
        }
    }
}