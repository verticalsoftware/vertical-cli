using CommunityToolkit.Diagnostics;

namespace Vertical.Cli;

/// <summary>
/// Represents the arity of an argument, option, or switch.
/// </summary>
public readonly struct Arity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Arity"/> type.
    /// </summary>
    /// <param name="minCount">The required minimum number of occurrences.</param>
    /// <param name="maxCount">The allowable maximum number of occurrences, or <c>null</c> if the maximum
    /// count is unconstrained.</param>
    public Arity(int minCount, int? maxCount)
    {
        Guard.IsGreaterThanOrEqualTo(minCount, 0);
        Guard.IsGreaterThanOrEqualTo(maxCount ?? int.MaxValue, Math.Max(1, minCount));
        
        MinCount = minCount;
        MaxCount = maxCount;
    }

    /// <summary>
    /// Gets the required minimum number of occurrences of an option or argument.
    /// </summary>
    public int MinCount { get; }

    /// <summary>
    /// Gets the allowable maximum number of occurrences of an option or argument.
    /// </summary>
    public int? MaxCount { get; }

    /// <summary>
    /// Gets whether max count indicates multiple value arity.
    /// </summary>
    public bool IsMultiValue => MaxCount is null or > 1;

    /// <inheritdoc />
    public override string ToString() => $"({MinCount}, {MaxCount?.ToString() ?? "-"})";

    /// <summary>
    /// Gets a <see cref="Arity"/> instance that is optional and allows no more than one occurrence.
    /// </summary>
    public static Arity ZeroOrOne => new(0, 1);

    /// <summary>
    /// Gets a <see cref="Arity"/> instance that is optional and allows multiple occurrences.
    /// </summary>
    public static Arity ZeroOrMany => new(0, null);

    /// <summary>
    /// Gets a <see cref="Arity"/> instance that allows exactly one occurrence.
    /// </summary>
    public static Arity One => new(1, 1);

    /// <summary>
    /// Gets a <see cref="Arity"/> instance that requires at least one occurrence.
    /// </summary>
    public static Arity OneOrMany => new(1, null);

    /// <summary>
    /// Gets a <see cref="Arity"/> instance that requires the specified number of occurrences.
    /// </summary>
    /// <param name="count">The number of occurrences to allow.</param>
    public static Arity Exactly(int count) => new(count, count);
}