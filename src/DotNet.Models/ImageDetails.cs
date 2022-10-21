// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Models;

public readonly record struct ImageDetails(
    string Image,
    string Tag,
    string TargetFrameworkMoniker,
    int LineNumber);
