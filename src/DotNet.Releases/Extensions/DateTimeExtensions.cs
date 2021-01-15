using System;

namespace DotNet.Versions.Extensions
{
    internal static class DateTimeExtensions
    {
        internal static bool IsInTheFuture(this DateTime dateTime) =>
            dateTime > DateTime.Now;
    }
}
