﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.GitHub;

public interface IResilientGitHubClientFactory
{
    IGitHubClient Create(string token);
}
