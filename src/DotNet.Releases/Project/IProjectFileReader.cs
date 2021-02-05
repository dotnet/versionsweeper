using DotNet.Models;
using System.Threading.Tasks;

namespace DotNet.Releases
{
    public interface IProjectFileReader
    {
        ValueTask<Project> ReadProjectAsync(string projectPath);
    }
}
