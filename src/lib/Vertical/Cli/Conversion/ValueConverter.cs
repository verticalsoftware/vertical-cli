namespace Vertical.Cli.Conversion;

/// <summary>
/// Converts values from string arguments to symbol value types.
/// </summary>
public abstract class ValueConverter
{
    /// <summary>
    /// Gets the type supported by the converter.
    /// </summary>
    public abstract Type ValueType { get; }
}

public abstract class ValueConverter<T> : ValueConverter
{
    /// <inheritdoc />
    public override Type ValueType => typeof(T);

    /// <summary>
    /// Converts the string argument representation to a <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="context">Conversion context.</param>
    /// <returns>The converted value.</returns>
    public abstract T Convert(ConversionContext<T> context);
}