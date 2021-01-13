using System.Text.Json;
using System.Text.RegularExpressions;

namespace DotNet.Extensions
{
    public class HyphenatedJsonNamingPolicy : JsonNamingPolicy
    {
        static string SplitCamelCase(string name) =>
            Regex.Replace(name, "([A-Z])", " $1", RegexOptions.Compiled).Trim();

        public override string ConvertName(string name) =>
            name.Contains("-") ? name.Replace("-", "") : SplitCamelCase(name).Replace(" ", "-");
    }
}
