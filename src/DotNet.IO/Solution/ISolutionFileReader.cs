using DotNet.Models;
using System.Threading.Tasks;

namespace DotNet.IO
{
    public interface ISolutionFileReader
    {
        ValueTask<Solution> ReadSolutionAsync(string solutionPath);
    }
}
