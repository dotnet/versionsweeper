// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Deployment.DotNet.Releases;

namespace DotNet.Models
{
    public class ReleaseFactory
    {
        public static IRelease Create<TSource>(
            TSource source,
            Func<TSource, string> toString,
            string tfm, SupportPhase supportPhase,
            DateTime? endOfLifeDate, string releaseNotesUrl) =>
            new ReleaseWrapper<TSource>(() => toString(source))
            {
                TargetFrameworkMoniker = tfm,
                SupportPhase = supportPhase,
                EndOfLifeDate = endOfLifeDate,
                ReleaseNotesUrl = releaseNotesUrl
            };

        private class ReleaseWrapper<TSource> : IRelease
        {
            private readonly Func<string> _toString = null!;

            public string TargetFrameworkMoniker { get; internal set; } = null!;
            public SupportPhase SupportPhase { get; internal set; }
            public DateTime? EndOfLifeDate { get; internal set; }
            public string ReleaseNotesUrl { get; internal set; } = null!;

            internal ReleaseWrapper(Func<string> toString) => _toString = toString;

            public string ToBrandString() => _toString.Invoke();
        }
    }
}
