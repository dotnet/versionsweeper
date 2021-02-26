// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Deployment.DotNet.Releases;

namespace DotNet.Models
{
    public interface IRelease
    {
        string TargetFrameworkMoniker { get; }
        SupportPhase SupportPhase { get; }
        DateTime? EndOfLifeDate { get; }
        string ReleaseNotesUrl { get; }

        string ToBrandString();
    }
}
