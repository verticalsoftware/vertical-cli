namespace Vertical.Cli.Utilities;

/// <summary>
/// Defines a pair of spans used in split operations.
/// </summary>
public readonly ref struct SplitSpan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SplitSpan"/> struct.
    /// </summary>
    /// <param name="span">The initial span used in a split operation.</param>
    public SplitSpan(ReadOnlySpan<char> span) : this(span, [])
    {
    }

    /// <summary>
    /// Gets an empty instance.
    /// </summary>
    public static SplitSpan Empty => new([]);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SplitSpan"/> struct.
    /// </summary>
    /// <param name="remainder">The trailing text remaining after a split operation.</param>
    /// <param name="slice">The leading text that was sliced from a split operation.</param>
    public SplitSpan(ReadOnlySpan<char> remainder, ReadOnlySpan<char> slice)
    {
        Remainder = remainder;
        Slice = slice;
    }

    /// <summary>
    /// Gets the trailing text remaining after a split operation.
    /// </summary>
    public ReadOnlySpan<char> Remainder { get; }

    /// <summary>
    /// Gets the leading text that was sliced from a split operation.
    /// </summary>
    public ReadOnlySpan<char> Slice { get; }

    /// <summary>
    /// Gets whether the remaining span is not empty.
    /// </summary>
    public bool HasRemainder => Remainder.Length > 0;

    /// <summary>
    /// Gets whether the sliced span is not empty.
    /// </summary>
    public bool HasSlice => Slice.Length > 0;

    /// <summary>
    /// Splits the remaining span to the specified width, optimally using a word boundary.
    /// </summary>
    /// <param name="width">Width to split.</param>
    /// <returns><see cref="SplitSpan"/></returns>
    public SplitSpan SplitToWidth(int width)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 0);

        var span = Remainder;
        
        if (width == 0)
        {
            return Empty;
        }

        --width;

        if (span.Length <= width)
        {
            return new SplitSpan([], span);
        }

        for (var pos = width; pos >= 0; pos--)
        {
            if (!char.IsWhiteSpace(span[pos]))
                continue;

            return new SplitSpan(span[pos..].Trim(), span[..pos]);
        }
        
        // Cannot split on work boundary
        return new SplitSpan(span[width..].Trim(), span[..width]);
    }
}