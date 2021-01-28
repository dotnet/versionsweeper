namespace DotNet.GitHubActions
{
    /// <summary>
    /// The code to exit an action
    /// </summary>
    public enum ExitCode
    {
        /// <summary>
        ///  A code indicating that the action was successful
        /// </summary>
        Success = 0,

        /// <summary>
        /// A code indicating that the action was a failure
        /// </summary>
        Failure = 1
    }
}
