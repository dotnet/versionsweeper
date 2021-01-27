using Microsoft.Extensions.FileSystemGlobbing;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.VersionSweeper
{
    public static class GlobbingExtensions
    {
        public static Matcher GetMatcher(this string[] ignoreGlobs, string searchPattern)
        {
            Matcher matcher = new();

            matcher.AddExcludePatterns(ignoreGlobs);
            matcher.AddIncludePatterns(
                searchPattern.AsMaskedExtensions().AsRecursivePatterns());

            return matcher;
        }

        public static IEnumerable<string> AsRecursivePatterns(this string[] patterns) =>
            patterns.Select(pattern => $"**/{pattern}");
     }
}
