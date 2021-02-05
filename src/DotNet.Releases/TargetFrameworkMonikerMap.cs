using System;
using System.Collections.Generic;
using DotNet.Extensions;

namespace DotNet.Releases
{
    static class TargetFrameworkMonikerMap
    {
        static readonly Dictionary<string, string> _frameworkMonikers =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["net11"] = "v1.1",
                ["net20"] = "v2.0",
                ["net35"] = "v3.5",
                ["net40"] = "v4.0",
                ["net403"] = "v4.0.3",
                ["net45"] = "v4.5",
                ["net451"] = "v4.5.1",
                ["net452"] = "v4.5.2",
                ["net46"] = "v4.6",
                ["net461"] = "v4.6.1",
                ["net462"] = "v4.6.2",
                ["net47"] = "v4.7",
                ["net471"] = "v4.7.1",
                ["net472"] = "v4.7.2",
                ["net48"] = "v4.8"
            }.AsReverseMap();

        static string StripAtHyphen(string tfm) => tfm?.Split("-")[0] ?? "";

        internal static bool RawMapsToKnown(string parsedTfm, string knownTfm)
        {
            if (_frameworkMonikers.TryGetValue(StripAtHyphen(parsedTfm), out var tfm))
            {
                return string.Equals(knownTfm, tfm, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(parsedTfm, knownTfm, StringComparison.OrdinalIgnoreCase);
            }

            return string.Equals(parsedTfm, knownTfm, StringComparison.OrdinalIgnoreCase);
        }
    }
}
