using DotNet.Models;
using System;

namespace DotNet.Versions.Extensions
{
    internal static class SupportPhaseExtensions
    {
        internal static bool IsSupported(
            this SupportPhase supportPhase, DateTime endOfLife) =>
            supportPhase switch
            {
                SupportPhase.Current => true,
                SupportPhase.LongTermSupport when endOfLife.IsInTheFuture() => true,

                _ => false
            };
    }
}
