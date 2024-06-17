using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines the usage arity of a command.
/// </summary>
public readonly struct Arity : IEquatable<Arity>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Arity"/> structure.
    /// </summary>
    /// <param name="minCount">The minimum number of usages expected.</param>
    /// <param name="maxCount">The maximum number of usages allowed.</param>
    public Arity(int minCount, int? maxCount = null)
    {
        Guard.IsGreaterThanOrEqualTo(minCount, 0, nameof(minCount));
        Guard.IsGreaterThanOrEqualTo(maxCount.GetValueOrDefault(int.MaxValue), minCount, nameof(maxCount));
        
        MinCount = minCount;
        MaxCount = maxCount;
    }

    /// <summary>
    /// Gets the minimum number of expected usages.
    /// </summary>
    public int MinCount { get; }

    /// <summary>
    /// Gets the maximum number of allowed usages.
    /// </summary>
    public int? MaxCount { get; }

    /// <inheritdoc />
    public override string ToString() => $"({MinCount},{MaxCount})";

    /// <summary>
    /// Returns an <see cref="Arity"/> value with a minimum and maximum count of one.
    /// </summary>
    /// <returns><see cref="Arity"/></returns>
    public static Arity One => new(1, 1);

    /// <summary>
    /// Returns an <see cref="Arity"/> instasnce with a minimum count of one.
    /// </summary>
    /// <returns><see cref="Arity"/></returns>
    public static Arity OneOrMany => new(1);
    
    /// <summary>
    /// Returns an <see cref="Arity"/> value with a minimum and maximum count of zero and one,
    /// respectively.
    /// </summary>
    /// <returns><see cref="Arity"/></returns>
    public static Arity ZeroOrOne => new(0, 1);

    /// <summary>
    /// Returns an <see cref="Arity"/> value with a minimum count of zero.
    /// </summary>
    /// <returns><see cref="Arity"/></returns>
    public static Arity ZeroOrMany => new(0);

    /// <summary>
    /// Returns an <see cref="Arity"/> value with a minimum and maximum count of the specified
    /// value.
    /// </summary>
    /// <param name="count">The arity value.</param>
    /// <returns><see cref="Arity"/></returns>
    public static Arity Exactly(int count) => new(count, count);

    /// <summary>
    /// Returns an <see cref="Arity"/> value with a minimum count of the specified
    /// value.
    /// </summary>
    /// <param name="count">The arity value.</param>
    /// <returns><see cref="Arity"/></returns>
    public static Arity AtLeast(int count) => new(count);

    /// <inheritdoc />
    public bool Equals(Arity other) => MinCount == other.MinCount && MaxCount == other.MaxCount;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Arity other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(MinCount, MaxCount);

    /// <summary>
    /// Determines equality
    /// </summary>
    public static bool operator ==(Arity x, Arity y) => x.Equals(y);
    
    /// <summary>
    /// Determines inequality
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator !=(Arity x, Arity y) => !x.Equals(y);
}