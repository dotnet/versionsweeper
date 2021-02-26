// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;

namespace DotNet.Extensions
{
    public static class StringExtensions
    {
        public static string? NullIfEmpty(this string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value;

        public static string EscapeUriString(this string? value) =>
            Uri.EscapeUriString(value ?? "");

        public static string? FirstAndLastSegmentOfPath(this string value, string separate)
        {
            var segments = value?.Split(Path.DirectorySeparatorChar) ?? Array.Empty<string>();
            return segments.Length switch
            {
                0 => default,
                1 => segments[0],
                _ => $"{segments[0]}/{separate}/{segments[^1]}"
            };
        }
    }
}
