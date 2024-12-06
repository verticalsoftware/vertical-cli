namespace Vertical.Cli.Utilities;

internal static class TypeExtensions
{
    public static IEnumerable<Type> GetTypeFamily(this Type type)
    {
        yield return type;

        for (var t = type.BaseType; t != null && t != typeof(object); t = t.BaseType)
        {
            yield return t;
        }
    }
}