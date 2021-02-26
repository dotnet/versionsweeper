// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Octokit;

namespace DotNet.GitHub
{
    public interface IGitHubIssueService
    {
        ValueTask<Issue> PostIssueAsync(
            string owner, string name, string token, NewIssue issue);
    }
}
