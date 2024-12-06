namespace Vertical.Cli.Conversion;

/// <summary>
/// Represents an object used to convert argument values to another type.
/// </summary>
public interface IValueConverter
{
     /// <summary>
     /// Gets the converter value type.
     /// </summary>
     Type ValueType { get; }
}

/// <summary>
/// Base type for value converters.
/// </summary>
/// <typeparam name="T">Value type being converted to.</typeparam>
public abstract class ValueConverter<T> : IValueConverter
{
     /// <inheritdoc />
     public Type ValueType => typeof(T);

     /// <summary>
     /// When implemented, converts the given string to a <typeparamref name="T"/> value.
     /// </summary>
     /// <param name="str">The string value to convert.</param>
     /// <returns>The converted value.</returns>
     /// <remarks>
     /// Implementations should throw exceptions if the conversion fails.
     /// </remarks>
     public abstract T Convert(string str);
}