namespace DotNet.GitHub
{
    public record GraphQLResult<T>
    {
        public Data<T>? Data { get; init; }
    }

    public record Data<T>
    {
        public Search<T>? Search { get; init; }
    }

    public record Search<T>
    {
        public T[]? Nodes { get; init; }
    }
}
