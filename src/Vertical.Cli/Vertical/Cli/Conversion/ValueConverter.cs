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