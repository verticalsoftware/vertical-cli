namespace Vertical.Cli.Utilities;

/// <summary>
/// Represents the span pair of a split operation.
/// </summary>
/// <param name="left">The split portion of the input span.</param>
/// <param name="right">The remaining portion of the input span.</param>
public readonly ref struct SpanPair(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
{
    /// <summary>
    /// Gets the split portion of the input span.
    /// </summary>
    public readonly ReadOnlySpan<char> Left = left;

    /// <summary>
    /// Gets the remaining portion of the input span.
    /// </summary>
    public readonly ReadOnlySpan<char> Right = right;

    /// <summary>
    /// Gets whether both left and right spans have zero length.
    /// </summary>
    public bool IsEmpty => Left.Length == 0 && Right.Length == 0;
    
    /// <summary>
    /// Gets an instance where both left and right spans are empty.
    /// </summary>
    public static SpanPair Empty => new SpanPair(ReadOnlySpan<char>.Empty, ReadOnlySpan<char>.Empty);

    /// <inheritdoc />
    public override string ToString() => $"Left='{Left}', Right='{Right}'";
}