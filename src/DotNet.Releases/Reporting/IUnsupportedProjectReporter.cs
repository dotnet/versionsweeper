﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

public interface IUnsupportedProjectReporter
{
    IAsyncEnumerable<ProjectSupportReport> ReportAsync(Project project, int outOfSupportWithinDays);
}
