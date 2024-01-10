namespace Vertical.Cli.SourceGenerator;

public static class Utilities
{
    public static void ForEach<T>(
        this IEnumerable<T> source,
        Action<T, int> action)
    {
        var count = 0;

        foreach (var item in source)
        {
            action(item, count++);
        }
    }
}