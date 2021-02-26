﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace DotNet.Releases.Extensions
{
    internal static class DateTimeExtensions
    {
        internal static bool IsInTheFuture(this DateTime dateTime) =>
            dateTime > DateTime.Now;
    }
}
