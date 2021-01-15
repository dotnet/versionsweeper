using System.Threading.Tasks;

namespace DotNet.Versions
{
    public interface ICoreReleaseIndexService
    {
        Task<CoreReleases?> GetReleaesAsync();
    }
}
