namespace Vertical.Cli.Utilities;

/// <summary>
/// Represents a value that is either an instance of <typeparamref name="T1"/> or <typeparamref name="T2"/>
/// </summary>
/// <typeparam name="T1">The value type</typeparam>
/// <typeparam name="T2">The alternate value type</typeparam>
public readonly struct Union<T1, T2>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Union{T1,T2}"/> struct.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public Union(T1 value) => Value = value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Union{T1,T2}"/> struct.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public Union(T2 value) => Value = value;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public object? Value { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Value}";

    /// <summary>
    /// Implicitly converts the given <typeparamref name="T1"/> value to a union type.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><see cref="Union{T1,T2}"/></returns>
    public static implicit operator Union<T1, T2>(T1 value) => new(value);
    
    /// <summary>
    /// Implicitly converts the given <typeparamref name="T2"/> value to a union type.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><see cref="Union{T1,T2}"/></returns>
    public static implicit operator Union<T1, T2>(T2 value) => new(value);
}