namespace Vertical.Cli.Utilities;

internal static class CollectionExtensions
{
    public static IEnumerable<(T item, int index)> SelectWithIndex<T>(this IEnumerable<T> source) =>
        source.Select((t, i) => (t, i));
    
    public static int SubMatchCountOf<T>(this IReadOnlyList<T> x, IReadOnlyList<T> y)
        where T : IEquatable<T>
    {
        var count = 0;
        var len = Math.Min(x.Count, y.Count);

        for (var c = 0; c < len; c++)
        {
            if (!x[c].Equals(y[c]))
                break;

            ++count;
        }

        return count;
    }

    public static int IndexOfAny<T>(this IReadOnlyList<T> source, IReadOnlyList<T> values)
        where T : IEquatable<T>
    {
        for (var c = 0; c < source.Count; c++)
        {
            if (source.Any(sourceValue => values.Any(value => value.Equals(sourceValue))))
                return c;
        }

        return -1;
    }

    public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        IEnumerable<TValue> values,
        Func<TValue, TKey> keySelector) where TKey : notnull
    {
        foreach (var value in values)
        {
            dictionary.Add(keySelector(value), value);
        }
    }
    
    public static void ReplaceRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        IEnumerable<TValue> values,
        Func<TValue, TKey> keySelector) where TKey : notnull
    {
        foreach (var value in values)
        {
            dictionary[keySelector(value)] = value;
        }
    }

    public static LinkedListNode<T>? Remove<T>(this LinkedList<T> linkedList,
        LinkedListNode<T> node,
        LinkedListNode<T>? sibling)
    {
        linkedList.Remove(node);
        return sibling;
    }
}