namespace Vertical.Cli.Conversion;

/// <summary>
/// Provides a <see cref="ValueConverter{T}"/> implementation for the <see cref="FileInfo"/> type.
/// </summary>
public sealed class FileInfoConverter : ValueConverter<FileInfo>
{
    /// <summary>
    /// Gets the default instance.
    /// </summary>
    public static readonly ValueConverter<FileInfo> Default = new FileInfoConverter();
    
    private FileInfoConverter()
    {
    }
    
    /// <inheritdoc />
    public override FileInfo Convert(string s) => new FileInfo(s);
}