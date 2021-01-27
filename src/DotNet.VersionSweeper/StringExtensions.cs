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
                    (true, _, _) => searchPattern.Split(';', StringSplitOptions.RemoveEmptyEntries),
                    (_, true, _) => searchPattern.Split('|', StringSplitOptions.RemoveEmptyEntries),
                    (_, _, true) => searchPattern.Split(',', StringSplitOptions.RemoveEmptyEntries),

                    _ => new string[] { searchPattern }
                };
    }
}
