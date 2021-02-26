// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Octokit;

namespace DotNet.GitHub
{
    public interface IGitHubLabelService
    {
        ValueTask<Label> GetOrCreateLabelAsync(
            string owner, string name, string token);
    }
}
