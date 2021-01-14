using System.Text.Json;
using System.Text.RegularExpressions;

namespace DotNet.Extensions
{
    public class HyphenatedJsonNamingPolicy : JsonNamingPolicy
    {
        static string SplitCamelCase(string name) =>
            Regex.Replace(name, "([A-Z])", " $1", RegexOptions.Compiled).Trim();

        /// <summary>
        /// The JSON for release notes has property names such as:
        /// "support-phase", "latest-sdk", and "channel-version"
        /// We need to remove these hyphens, likewise; we need to also take camel cased property names
        /// from C# object graphs and replace them with a hypens.
        /// </summary>
        public override string ConvertName(string name) =>
            name.Contains("-") ? name.Replace("-", "") : SplitCamelCase(name).Replace(" ", "-");
    }
}
