// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases.Extensions;

public static class ProductExtensions
{
    public static string GetTargetFrameworkMoniker(this Product product) =>
        product is not null
            ? product.ProductName switch
            {
                ".NET" => $"net{product.ProductVersion}",
                ".NET Core" => $"netcoreapp{product.ProductVersion}",
                _ => product.ProductVersion
            }
            : string.Empty;
}
