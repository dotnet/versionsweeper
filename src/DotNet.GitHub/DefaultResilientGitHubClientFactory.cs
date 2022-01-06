// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public class DefaultResilientGitHubClientFactory : IResilientGitHubClientFactory
{
    readonly ResilientGitHubClientFactory _clientFactory;

    public DefaultResilientGitHubClientFactory(
        ResilientGitHubClientFactory clientFactory) =>
        _clientFactory = clientFactory;

    public IGitHubClient Create(string token) =>
        _clientFactory.Create(
            productHeaderValue: GitHubProduct.Header,
            credentials: new(token));
}
