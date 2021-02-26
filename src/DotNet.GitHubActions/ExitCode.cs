// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
