using DotNet.Models;
using System.Collections.Generic;

namespace DotNet.Releases
{
    public interface IUnsupportedProjectReporter
    {
        IAsyncEnumerable<ProjectSupportReport> ReportAsync(Project project);
    }
}
