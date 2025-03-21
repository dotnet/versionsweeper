﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DotNet.Releases;
using Xunit;

namespace DotNet.ReleasesTests;

public sealed class LabeledVersionTests
{
    public static IEnumerable<object[]> AsLabeledVersionInput =
    [
            ["5.0", new LabeledVersion(new Version(5, 0))],
            ["5.0.2", new LabeledVersion(new Version(5, 0, 2))],
            ["3.1.405", new LabeledVersion(new Version(3, 1, 405))],
            ["2.2.207", new LabeledVersion(new Version(2, 2, 207))],
            ["8.1.0.314-preview1", new LabeledVersion(new Version(8, 1, 0, 314), "preview1")],
            ["7.7.84.34-beta", new LabeledVersion(new Version(7, 7, 84, 34), "beta")],
            ["3.5.0-sp1", new LabeledVersion(new Version(3, 5, 0), "sp1")],
            ["pickles", new LabeledVersion(null)]
        ];

    [
        Theory,
        MemberData(nameof(AsLabeledVersionInput))
    ]
    public void AsLabeledVersionTest(string version, LabeledVersion expectedVersion) =>
        Assert.Equal(expectedVersion, version);
}
