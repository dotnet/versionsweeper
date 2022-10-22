// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public sealed class DefaultResilientGitHubClientFactory : IResilientGitHubClientFactory
{
    public IGitHubClient Create(string token) =>
        new GitHubClient(
            productInformation: GitHubProduct.Header,
            credentialStore: new InMemoryCredentialStore(new(token)));
}
