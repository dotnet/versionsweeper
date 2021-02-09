using Octokit;
using System.Threading.Tasks;

namespace DotNet.GitHub
{
    public interface IGitHubLabelService
    {
        ValueTask<Label> GetOrCreateLabelAsync(
            string owner, string name, string token);
    }
}
