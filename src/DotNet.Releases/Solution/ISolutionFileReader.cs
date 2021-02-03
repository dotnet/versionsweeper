using System.Threading.Tasks;

namespace DotNet.Releases.Solution
{
    public interface ISolutionFileReader
    {
        ValueTask<Solution> ReadSolutionAsync(string solutionPath);
    }
}
