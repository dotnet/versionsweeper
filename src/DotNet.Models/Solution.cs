using System.Collections.Generic;

namespace DotNet.Models
{
    public class Solution
    {
        public string FullPath { get; set; } = null!;

        public HashSet<Project> Projects { get; } = new();
    }
}
