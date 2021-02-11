using DotNet.Models;
using System.Threading.Tasks;

namespace DotNet.IO
{
    public interface IProjectFileReader
    {
        ValueTask<Project> ReadProjectAsync(string projectPath);
    }
}
