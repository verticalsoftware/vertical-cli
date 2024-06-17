namespace Vertical.Cli.Conversion;

/// <summary>
/// Provides a <see cref="ValueConverter{T}"/> implementation that returns the input string.
/// </summary>
public sealed class StringConverter : ValueConverter<string>
{
    /// <summary>
    /// Defines the default instance.
    /// </summary>
    public static readonly ValueConverter<string> Default = new StringConverter();
    
    private StringConverter()
    {
    }
    
    /// <inheritdoc />
    public override string Convert(string s) => s;
}