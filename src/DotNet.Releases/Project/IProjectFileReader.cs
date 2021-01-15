using System.Threading.Tasks;

namespace DotNet.Versions
{
    public interface IProjectFileReader
    {
        Task<string[]> ReadProjectTfmsAsync(string filePath);
    }
}
