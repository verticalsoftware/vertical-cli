namespace Vertical.Cli.Configuration;

/// <summary>
/// Composes arity values for collections.
/// </summary>
public sealed class CollectionArity
{
    private CollectionArity(Arity arity)
    {
        Arity = arity;
    }

    /// <summary>
    /// Gets the arity.
    /// </summary>
    public Arity Arity { get; }

    /// <summary>
    /// Defines an arity value that allows zero or more values.
    /// </summary>
    public static readonly CollectionArity ZeroOrMore = new(new Arity(0, null));
    
    /// <summary>
    /// Defines an arity value that requires at least one value.
    /// </summary>
    public static readonly CollectionArity OneOrMore = new(new Arity(1, null));
        
    /// <summary>
    /// Creates an arity value that requires exactly <paramref name="count"/> values.
    /// </summary>
    /// <param name="count">The number of values to require.</param>
    /// <returns><see cref="CollectionArity"/></returns>
    public static CollectionArity Exactly(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        return new CollectionArity(new Arity(count, count));
    }
    
    /// <summary>
    /// Creates an arity value that requires <paramref name="count"/> values.
    /// </summary>
    /// <param name="count">The minimum number of values to require.</param>
    /// <returns><see cref="CollectionArity"/></returns>
    public static CollectionArity AtLeast(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        return new CollectionArity(new Arity(count, null));
    }
    
    /// <summary>
    /// Creates an arity value that allows up to <paramref name="count"/> values.
    /// </summary>
    /// <param name="count">The maximum number of values to allow.</param>
    /// <returns><see cref="CollectionArity"/></returns>
    public static CollectionArity AtMost(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        return new CollectionArity(new Arity(0, count));
    }
}