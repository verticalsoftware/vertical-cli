namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines the required and allowed arity of the usage of a symbol.
/// </summary>
public readonly struct Arity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Arity"/> struct.
    /// </summary>
    /// <param name="minimum">The required argument count.</param>
    /// <param name="maximum">The allowed argument count.</param>
    public Arity(int minimum, int? maximum)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(minimum, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(maximum ?? int.MaxValue, minimum);
        ArgumentOutOfRangeException.ThrowIfLessThan(maximum ?? int.MaxValue, 1);
        
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// Gets the minimum argument count.
    /// </summary>
    public int Minimum { get; }

    /// <summary>
    /// Gets the maximum argument count.
    /// </summary>
    public int? Maximum { get; }

    /// <summary>
    /// Gets whether the arity represents an optional symbol.
    /// </summary>
    public bool IsOptional => Minimum == 0;

    /// <summary>
    /// Gets whether the arity is variadic.
    /// </summary>
    public bool IsVariadic => Maximum is null;
    
    /// <inheritdoc />
    public override string ToString() => Maximum is null
        ? $"({Minimum}-many)"
        : $"({Minimum}-{Maximum})";

    /// <summary>
    /// Creates a new instance that allows no more than one argument.
    /// </summary>
    public static Arity ZeroOrOne => new(0, 1);

    /// <summary>
    /// Creates a new instance that allows an unrestricted number of arguments.
    /// </summary>
    public static Arity ZeroOrMore => new(0, null);

    /// <summary>
    /// Creates a new instance that requires one argument.
    /// </summary>
    public static Arity One => new(1, 1);

    /// <summary>
    /// Creates a new instance that requires a minimum of one argument.
    /// </summary>
    public static Arity OneOrMore => new(1, null);

    public void Deconstruct(out int min, out int max)
    {
        min = Minimum;
        max = Maximum ?? int.MaxValue;
    }
}