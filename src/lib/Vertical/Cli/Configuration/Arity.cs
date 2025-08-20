namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the required or acceptable count of usages of a symbol.
/// </summary>
public sealed class Arity
{
    internal Arity(int minCount, int? maxCount)
    {
        MinCount = minCount;
        MaxCount = maxCount;
    }

    internal static readonly Arity ZeroOrOne = new(0, 1);
    internal static readonly Arity ZeroOrMore = new(0, null);
    internal static readonly Arity One = new(1, 1);
    internal static readonly Arity OneOrMore = new(1, null);
    
    
    /// <summary>
    /// Gets the minimum count.
    /// </summary>
    public int MinCount { get; }

    /// <summary>
    /// Gets the maximum count, or <c>null</c> if unconstrained.
    /// </summary>
    public int? MaxCount { get; }

    /// <inheritdoc />
    public override string ToString() => $"({MinCount}, {MaxCountString})";

    /// <summary>
    /// Determines whether the given count satisfies the arity.
    /// </summary>
    /// <param name="count">Count of received values.</param>
    /// <returns>
    /// <c>true</c> if count is greater or equal to the minimum count and less or equal to the
    /// maximum count.
    /// </returns>
    public bool IsCountValid(int count)
    {
        return count >= MinCount
               &&
               (MaxCount is null || count <= MaxCount);
    }

    private string MaxCountString => MaxCount.HasValue ? $"{MaxCount}" : "*";
}