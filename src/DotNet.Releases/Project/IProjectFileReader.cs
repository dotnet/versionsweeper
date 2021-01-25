using System.Threading.Tasks;

namespace DotNet.Releases
{
    public interface IProjectFileReader
    {
        Task<string[]> ReadProjectTfmsAsync(string filePath);
    }
}
