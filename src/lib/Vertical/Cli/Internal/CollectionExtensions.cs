namespace Vertical.Cli.Internal;

internal static class CollectionExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> provideValue)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
            return value;

        value = provideValue();
        dictionary.Add(key, value);

        return value;
    }

    public static LinkedListNode<T>? Dequeue<T>(this LinkedList<T> list, LinkedListNode<T> node)
    {
        var next = node.Next;

        list.Remove(node);
        return next;
    }

    public static IEnumerable<LinkedListNode<T>> SelectNodes<T>(this LinkedList<T> list)
    {
        for (var node = list.First; node != null; node = node.Next)
        {
            yield return node;
        }
    }

    public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            stack.Push(item);
        }
    }
}