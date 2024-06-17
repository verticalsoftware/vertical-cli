namespace Vertical.Cli.Conversion;

/// <summary>
/// Provides a <see cref="ValueConverter{T}"/> implementation for the <see cref="DirectoryInfo"/> type.
/// </summary>
public sealed class DirectoryInfoConverter : ValueConverter<DirectoryInfo>
{
    /// <summary>
    /// Gets the default instance.
    /// </summary>
    public static readonly ValueConverter<DirectoryInfo> Default = new DirectoryInfoConverter();
    
    private DirectoryInfoConverter()
    {
    }
    
    /// <inheritdoc />
    public override DirectoryInfo Convert(string s) => new DirectoryInfo(s);
}