namespace Vertical.Cli.Internal;

internal static class Utilities
{
    internal static IEnumerable<TValue> SelectRecursive<TSource, TValue>(
        this TSource source,
        Func<TSource, (TValue, TSource?)> selector)
    {
        var target = source;
        while (target != null)
        {
            var result = selector(target);
            yield return result.Item1;
            target = result.Item2;
        }
    }
    
    internal static IEnumerable<TValue> SelectManyRecursive<TSource, TValue>(
        this TSource source,
        Func<TSource, (IEnumerable<TValue>, TSource?)> selector)
    {
        var target = source;
        while (target != null)
        {
            var result = selector(target);
            foreach (var item in result.Item1)
                yield return item;
            target = result.Item2;
        }
    }
}