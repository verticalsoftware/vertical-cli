using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// Defines methods used to trim and split strings.
/// </summary>
public static class LayoutUtilities
{
    /// <summary>
    /// Splits a span by the given length.
    /// </summary>
    /// <param name="span">The span to split.</param>
    /// <param name="length">The maximum number of characters to allow in the left span.</param>
    /// <returns><see cref="SpanPair"/></returns>
    public static SpanPair SplitToLength(this ReadOnlySpan<char> span, int length)
    {
        return span.Length <= length
            ? new SpanPair(span, ReadOnlySpan<char>.Empty)
            : new SpanPair(span[..length], span[length..]);
    }

    /// <summary>
    /// Splits a span by the given length, consider newline characters.
    /// </summary>
    /// <param name="str">The string to split.</param>
    /// <param name="length">The maximum number of characters to allow in the left span.</param>
    /// <returns><see cref="SpanPair"/></returns>
    public static SpanPair SplitLinesToLength(this string str, int length) => str.AsSpan().SplitLinesToLength(length);

    /// <summary>
    /// Splits a span by the given length, consider newline characters.
    /// </summary>
    /// <param name="span">The span to split.</param>
    /// <param name="length">The maximum number of characters to allow in the left span.</param>
    /// <returns><see cref="SpanPair"/></returns>
    public static SpanPair SplitLinesToLength(this ReadOnlySpan<char> span, int length)
    {
        if (span.Length == 0)
        {
            return SpanPair.Empty;
        }

        // if (span.Length < length)
        // {
        //     return new SpanPair(span, ReadOnlySpan<char>.Empty);
        // }

        var splitPosition = -1;
        var minLength = Math.Min(span.Length, length);

        for (var c = 0; c < minLength; c++)
        {
            switch (span[c])
            {
                case '\n':
                    return new SpanPair(span[..c], span[(c + 1)..]);
                
                case ' ':
                    splitPosition = c;
                    break;
            }
        }

        if (span.Length < length)
        {
            return new SpanPair(span, ReadOnlySpan<char>.Empty);
        }

        return splitPosition > -1 
            ? new SpanPair(span[..splitPosition], span[++splitPosition..]) 
            :
            // String is not splittable on whitespace
            new SpanPair(span[..minLength], span[minLength..]);
    }
    
    /// <summary>
    /// Resplits a <see cref="SpanPair"/> on its right span.
    /// </summary>
    /// <param name="spanPair">The span pair.</param>
    /// <param name="length">The maximum number of characters to allow in the left span.</param>
    /// <returns><see cref="SpanPair"/></returns>
    public static SpanPair ResplitLinesToLength(this SpanPair spanPair, int length)
    {
        return spanPair.Right.SplitLinesToLength(length);
    }
}