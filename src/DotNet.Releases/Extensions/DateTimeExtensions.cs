using System;

namespace DotNet.Releases.Extensions
{
    internal static class DateTimeExtensions
    {
        internal static bool IsInTheFuture(this DateTime dateTime) =>
            dateTime > DateTime.Now;
    }
}
