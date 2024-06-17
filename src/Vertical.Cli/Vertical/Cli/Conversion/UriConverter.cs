namespace Vertical.Cli.Conversion;

/// <summary>
/// Implements a converter that creates <see cref="Uri"/> instances.
/// </summary>
public sealed class UriConverter : ValueConverter<Uri>
{
    /// <summary>
    /// Defines the default instance.
    /// </summary>
    public static readonly ValueConverter<Uri> Default = new UriConverter();
    
    private UriConverter()
    {
    }
    
    /// <inheritdoc />
    public override Uri Convert(string s) => new(s, UriKind.RelativeOrAbsolute);
}