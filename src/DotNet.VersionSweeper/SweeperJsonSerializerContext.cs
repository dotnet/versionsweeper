// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

[JsonSourceGenerationOptions(
    defaults: JsonSerializerDefaults.Web,
    WriteIndented = true,
    UseStringEnumConverter = true,
    AllowTrailingCommas = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNameCaseInsensitive = false,
    IncludeFields = true)]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(VersionSweeperConfig))]
internal partial class SweeperJsonSerializerContext : JsonSerializerContext;
