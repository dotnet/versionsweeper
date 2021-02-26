﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace DotNet.Releases
{
    public interface IFrameworkReleaseIndexService
    {
        HashSet<string> FrameworkReseaseFileNames { get; }
    }
}
