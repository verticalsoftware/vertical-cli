namespace Vertical.Cli.Utilities;

internal static class SupportedCollectionTypes
{
    private static readonly HashSet<Type> Entries = new()
    {
        typeof(List<>),
        typeof(IList<>),
        typeof(IReadOnlyList<>),
        typeof(ICollection<>),
        typeof(IReadOnlyCollection<>),
        typeof(IEnumerable<>),
        typeof(ISet<>),
        typeof(LinkedList<>),
        typeof(HashSet<>),
        typeof(SortedSet<>),
        typeof(Stack<>),
        typeof(Queue<>),
#if NET6_0_OR_GREATER
        typeof(IReadOnlySet<>)
#endif
    };

    internal static bool Contains(Type type) => Entries.Contains(type);
}