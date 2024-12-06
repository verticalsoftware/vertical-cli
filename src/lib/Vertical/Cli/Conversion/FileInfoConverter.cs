namespace Vertical.Cli.Conversion;

/// <summary>
/// Converts arguments to <see cref="DirectoryInfo"/>
/// </summary>
public sealed class FileInfoConverter : ValueConverter<FileInfo>
{
    /// <inheritdoc />
    public override FileInfo Convert(string str) => new(str);
}