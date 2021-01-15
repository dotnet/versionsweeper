using System.Collections.Generic;

namespace DotNet.Versions
{
    public interface IUnsupportedProjectReporter
    {
        IAsyncEnumerable<ProjectSupportReport> ReportAsync(string projectPath, string[] tfms);
    }
}
