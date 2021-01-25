using System.Threading.Tasks;

namespace DotNet.Releases
{
    public interface IProjectFileReader
    {
        Task<(int LineNumber, string[] Tfms)> ReadProjectTfmsAsync(string filePath);
    }
}
