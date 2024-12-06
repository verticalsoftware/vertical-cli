namespace Vertical.Cli.Conversion;

/// <summary>
/// Converts arguments to <see cref="DirectoryInfo"/>
/// </summary>
public sealed class DirectoryInfoConverter : ValueConverter<DirectoryInfo>
{
    /// <inheritdoc />
    public override DirectoryInfo Convert(string str) => new(str);
}