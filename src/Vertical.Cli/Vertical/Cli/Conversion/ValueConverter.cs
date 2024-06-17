namespace Vertical.Cli.Conversion;

/// <summary>
/// Super type for all value converters.
/// </summary>
public abstract class ValueConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueConverter"/> type.
    /// </summary>
    /// <param name="targetType">Target type.</param>
    protected ValueConverter(Type targetType) => TargetType = targetType;

    /// <summary>
    /// Gets the target type.
    /// </summary>
    public Type TargetType { get; }

    /// <summary>
    /// Creates a <see cref="ValueConverter{T}"/> instance that uses a delegate
    /// function.
    /// </summary>
    /// <param name="function">Function that accepts a string input and converts the value
    /// to <typeparamref name="T"/></param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns><see cref="ValueConverter{T}"/></returns>
    public static ValueConverter<T> Create<T>(Func<string, T> function) =>
        new ValueConverter<T>.Inline(function);
}

/// <summary>
/// Represents an object that converts string values to other forms.
/// </summary>
/// <typeparam name="T">The type of value to convert.</typeparam>
public abstract class ValueConverter<T> : ValueConverter
{
    internal sealed class Inline(Func<string, T> function) : ValueConverter<T>
    {
        /// <inheritdoc />
        public override T Convert(string s) => function(s);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueConverter{T}"/> type.
    /// </summary>
    protected ValueConverter() : base(typeof(T))
    {
    }
    
    /// <summary>
    /// Converts the string value to the target type.
    /// </summary>
    /// <param name="s">String value to convert.</param>
    /// <returns>A value of <typeparamref name="T"></typeparamref></returns>
    public abstract T Convert(string s);
}