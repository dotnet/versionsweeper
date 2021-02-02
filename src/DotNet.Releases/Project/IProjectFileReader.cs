using System.Threading.Tasks;

namespace DotNet.Releases
{
    public interface IProjectFileReader
    {
        ValueTask<(int LineNumber, string[] Tfms)> ReadProjectTfmsAsync(string filePath);
    }
}
