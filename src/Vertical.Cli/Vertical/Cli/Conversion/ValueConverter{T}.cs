namespace Vertical.Cli.Conversion;

/// <summary>
/// Converts string arguments to <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
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