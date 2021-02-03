using System;
using System.Collections.Generic;

namespace DotNet.Releases.Solution
{
    public class Solution
    {
        public string Path { get; set; } = null!;

        /// <summary>
        /// The project mapping, where the key is the project name and 
        /// the value is the project's fully qualified path.
        /// </summary>
        public Dictionary<string, string> Projects { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
