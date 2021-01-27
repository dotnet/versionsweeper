using Octokit;
using System;

namespace DotNet.GitHub
{
    public record ExistingIssue
    {
        public string? Title { get; init; }
        public long Number { get; init; }
        public string? Url { get; init; }
        public ItemState State { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateTime? CreatedAt { get; init; }
        public DateTime? ClosedAt { get; init; }
    }
}
