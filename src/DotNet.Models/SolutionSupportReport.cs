using System.Collections.Generic;

namespace DotNet.Models
{
    public record SolutionSupportReport(Solution Solution)
    {
        public HashSet<ProjectSupportReport> ProjectSupportReports { get; } = new();
    }
}
