namespace Vertical.Cli.Utilities;

internal static class CollectionExtensions
{
    extension<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        public TValue GetOrAdd(TKey key, Func<TValue> factory)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;

            value = factory();
            dictionary.Add(key, value);
            return value;
        }
    }

    extension<T>(List<T> list)
    {
        public int AddRangeWithCountTracking(IEnumerable<T> items)
        {
            var count = list.Count;
            list.AddRange(items);
            return list.Count - count;
        }
    }

    extension<T>(IEnumerable<T> items)
    {
        public Queue<T> ToQueue() => new(items);

        public void Visit(Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }

    extension<T>(HashSet<T> hashSet)
    {
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                hashSet.Add(item);
            }
        }
    }

    extension<T>(Stack<T> stack)
    {
        public void PushRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                stack.Push(item);
            }
        }
    }
}