// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Models;

public interface IRepoOptions
{
    string Owner { get; }
    string Name { get; }
    string Branch { get; }
    string Directory { get; }
}
