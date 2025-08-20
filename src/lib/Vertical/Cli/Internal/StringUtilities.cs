namespace Vertical.Cli.Internal;

internal static class StringUtilities
{
    public static string ToKebabCase(this string str, ReadOnlySpan<char> prefix)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;
        
        Span<char> span = stackalloc char[str.Length * 2 + prefix.Length];
        
        prefix.CopyTo(span);
        var i = prefix.Length;

        for (var c = 0; c < str.Length; c++)
        {
            var character = str[c];

            if (c > 0 && char.IsUpper(character))
            {
                span[i++] = '-';
            }

            span[i++] = char.ToLower(character);
        }

        return new string(span[..i]);
    }

    public static string ToUpperSnakeCase(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        Span<char> span = stackalloc char[str.Length * 2];
        var i = 1;

        span[0] = char.ToUpper(str[0]);

        for (var c = 1; c < str.Length; c++)
        {
            var character = str[c];

            if (char.IsUpper(character))
            {
                span[i++] = '_';
            }

            span[i++] = char.ToUpper(character);
        }

        return new string(span[..i]);
    }

    public static string ToCamelCase(this string str, ReadOnlySpan<char> prefix)
    {
        ArgumentNullException.ThrowIfNull(str);

        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        Span<char> span = stackalloc char[str.Length + prefix.Length];
        prefix.CopyTo(span);

        var i = prefix.Length;

        span[i++] = char.ToLower(str[0]);

        for (var c = 1; c < str.Length; c++)
        {
            span[i++] = str[c];
        }

        return new string(span);
    }
}