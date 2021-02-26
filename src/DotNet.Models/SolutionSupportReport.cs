﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace DotNet.Models
{
    public record SolutionSupportReport(Solution Solution)
    {
        public HashSet<ProjectSupportReport> ProjectSupportReports { get; } = new();
    }
}
