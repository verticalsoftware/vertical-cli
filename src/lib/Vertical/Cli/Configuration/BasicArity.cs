namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines the arity of a single-valued option or argument.
/// </summary>
public sealed class BasicArity
{
    private BasicArity(Arity arity)
    {
        Arity = arity;
    }

    /// <summary>
    /// Gets the arity value.
    /// </summary>
    public Arity Arity { get; }

    /// <summary>
    /// Gets an <see cref="Arity"/> that accepts a single value at most.
    /// </summary>
    public static BasicArity ZeroOrOne => new(Arity.ZeroOrOne);

    /// <summary>
    /// Gets an <see cref="Arity"/> that requires a single value.
    /// </summary>
    public static BasicArity One => new(Arity.One);
}