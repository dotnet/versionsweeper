using Octokit;

namespace DotNet.GitHub
{
    static class Product
    {
        static readonly string _name = "DotNetVersionSweeper";
        static readonly string _version = "1.0";

        internal static ProductHeaderValue Header { get; } = new(_name, _version);
    }
}
