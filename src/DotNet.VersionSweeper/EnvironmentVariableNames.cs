namespace DotNet.VersionSweeper
{
    public static class EnvironmentVariableNames
    {
        public static class GitHub
        {
            public const string Token = "GITHUB_TOKEN";
        }
        
        public static class Sweeper
        {
            public const string Owner = "DOTNET_VERSIONSWEEPER_OWNER";
            public const string Name = "DOTNET_VERSIONSWEEPER_NAME";
            public const string Branch = "DOTNET_VERSIONSWEEPER_BRANCH";
            public const string Directory = "DOTNET_VERSIONSWEEPER_DIRECTORY";
            public const string SearchPattern = "DOTNET_VERSIONSWEEPER_PATTERN";
        }
    }
}
