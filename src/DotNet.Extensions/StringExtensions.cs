// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNet.Extensions
{
    public static class StringExtensions
    {
        public static string? NullIfEmpty(this string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value;

        public static string EscapeUriString(this string? value) =>
            Uri.EscapeDataString(value ?? "");

        // Inspired and adapted from: https://stackoverflow.com/a/51282271/2410379
        public static string? ShrinkPath(
            this string value, string spacer = "...", int limit = 125)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            if (value.Length <= limit)
            {
                return value.Replace('\\', '/');
            }

            var file = new FileInfo(value);
            var segments = value.Split(Path.DirectorySeparatorChar);
            var parts = new List<string>
            {
                segments[0],
                spacer,
                segments[^1]
            };

            StringBuilder result = new(string.Join('/', parts));

            var dir = file.Directory;
            while (result.Length < limit && dir is not null)
            {
                if (result.Length + dir.Name.Length > limit)
                {
                    break;
                }

                parts.Insert(2, dir.Name);

                dir = dir.Parent;
                result = new(string.Join('/', parts));
            }

            return result.ToString();
        }
    }
}
