using Xunit;
using System.Collections.Generic;
using System;

namespace DotNet.Releases.Tests
{
    public class LabeledVersionTests
    {
        public static IEnumerable<object[]> AsLabeledVersionInput = new[]
{
            new object[] { "5.0",     new LabeledVersion(new Version(5, 0)) },
            new object[] { "5.0.2",   new LabeledVersion(new Version(5, 0, 2)) },
            new object[] { "3.1.405", new LabeledVersion(new Version(3, 1, 405)) },
            new object[] { "2.2.207", new LabeledVersion(new Version(2, 2, 207)) },
            new object[] { "3.5.0-sp1", new LabeledVersion(new Version(3, 5, 0), "sp1") },
            new object[] { "pickles", new LabeledVersion(null) }
        };

        [
            Theory,
            MemberData(nameof(AsLabeledVersionInput))
        ]
        public void AsLabeledVersionTest(string version, LabeledVersion expectedVersion) =>
            Assert.Equal(expectedVersion, version);
    }
}