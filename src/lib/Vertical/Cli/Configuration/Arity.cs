namespace Vertical.Cli.Configuration;

/// <summary>
/// Describes the minimum and maximum number of values for a parameter.
/// </summary>
public readonly struct Arity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Arity"/> struct.
    /// </summary>
    /// <param name="minCount">The minimum number of values.</param>
    /// <param name="maxCount">The maximum number of values, or <c>null</c> if unconstrained.</param>
    /// <exception cref="ArgumentException"><paramref name="minCount"/> is less than 0.</exception>
    /// <exception cref="ArgumentException"><paramref name="maxCount"/> is less than <paramref name="minCount"/>.</exception>
    public Arity(int minCount, int? maxCount)
    {
        if (minCount < 0)
        {
            throw new ArgumentException("minCount cannot be less than 0", nameof(minCount));
        }

        if (maxCount < minCount)
        {
            throw new ArgumentException("maxCount cannot be less than min count", nameof(minCount));
        }

        MinCount = minCount;
        MaxCount = maxCount;
    }
    
    /// <summary>
    /// Gets the minimum number of values.
    /// </summary>
    public int MinCount { get; } = 0;
    
    /// <summary>
    /// Gets the maximum number of parameters, or <c>null</c> if the count is unconstrained.
    /// </summary>
    public int? MaxCount { get; } = 1;

    /// <inheritdoc />
    public override string ToString() => $"({MinCount}, {MaxCount})";

    /// <summary>
    /// Defines an arity with a minimum count of 0 and a maximum count of 1.
    /// </summary>
    public static Arity ZeroOrOne => new(0, 1);
    
    /// <summary>
    /// Defines an arity with a minimum count of 0.
    /// </summary>
    public static Arity ZeroOrMany => new(0, null);

    /// <summary>
    /// Defines an arity with a minimum and maximum count of 1.
    /// </summary>
    public static Arity One => new(1, 1);

    /// <summary>
    /// Defines an arity with a minimum count of 1.
    /// </summary>
    public static Arity OneOrMany => new(1, null);
    
    /// <summary>
    /// Defines an arity with matching minimum and maximum count.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static Arity Exactly(int count) => new(count, count);
}