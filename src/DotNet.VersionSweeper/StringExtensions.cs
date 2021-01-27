using System;

namespace DotNet.VersionSweeper
{
    public static class StringExtensions
    {
        public static string[] AsMaskedExtensions(this string searchPattern) =>
            searchPattern is null or { Length: 0 }
                ? Array.Empty<string>()
                : (searchPattern.Contains(';'), searchPattern.Contains('|'), searchPattern.Contains(',')) switch
                {
                    (true, false, false) => searchPattern.Split(';', StringSplitOptions.RemoveEmptyEntries),
                    (false, true, false) => searchPattern.Split('|', StringSplitOptions.RemoveEmptyEntries),
                    (false, false, true) => searchPattern.Split(',', StringSplitOptions.RemoveEmptyEntries),

                    _ => new string[] { searchPattern }
                };
    }
}
