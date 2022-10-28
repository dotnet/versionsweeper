// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DotNet.Releases;

public sealed class LabeledVersion : IComparable<LabeledVersion?>
{
    private readonly Version? _parsedVersion;

    public int Build => _parsedVersion?.Build ?? 0;
    public int Major => _parsedVersion?.Major ?? 0;
    public short MajorRevision => _parsedVersion?.MajorRevision ?? 0;
    public int Minor => _parsedVersion?.Minor ?? 0;
    public short MinorRevision => _parsedVersion?.MinorRevision ?? 0;
    public int Revision => _parsedVersion?.Revision ?? 0;
    public string? Label { get; private set; }

    private LabeledVersion() { }

    public LabeledVersion(Version version, string? label = null) =>
        (_parsedVersion, Label) = (version, label);

    public static bool TryParse(string? input, out LabeledVersion? labeledVersion)
    {
        if (input is { Length: > 0 })
        {
            var split = input.Split("-");
            if (Version.TryParse(split[0], out var version))
            {
                labeledVersion = new LabeledVersion(version, split.Length > 1 ? split[1] : null);
                return true;
            }
        }

        labeledVersion = null;
        return false;
    }

    public static implicit operator LabeledVersion(string input) =>
        TryParse(input, out var version) ? version ?? new() : new();

    public static bool operator ==(LabeledVersion? v1, LabeledVersion? v2) =>
        v1?._parsedVersion == v2?._parsedVersion;

    public static bool operator >(LabeledVersion? v1, LabeledVersion? v2) =>
        v1?._parsedVersion > v2?._parsedVersion;

    public static bool operator >=(LabeledVersion? v1, LabeledVersion? v2) =>
        v1?._parsedVersion >= v2?._parsedVersion;

    public static bool operator !=(LabeledVersion? v1, LabeledVersion? v2) =>
        v1?._parsedVersion != v2?._parsedVersion;

    public static bool operator <(LabeledVersion? v1, LabeledVersion? v2) =>
        v1?._parsedVersion < v2?._parsedVersion;

    public static bool operator <=(LabeledVersion? v1, LabeledVersion? v2) =>
        v1?._parsedVersion <= v2?._parsedVersion;

    int IComparable<LabeledVersion?>.CompareTo(LabeledVersion? other) =>
        _parsedVersion?.CompareTo(other?._parsedVersion) ?? 0 +
        Label?.CompareTo(other?.Label) ?? 0;

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj)
        || obj is not null
        && obj is LabeledVersion other
        && _parsedVersion == other._parsedVersion
        && Label == other.Label;

    public override int GetHashCode() =>
        _parsedVersion?.GetHashCode() ?? 0 + Label?.GetHashCode() ?? 0;
}
