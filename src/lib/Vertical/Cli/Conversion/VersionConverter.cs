namespace Vertical.Cli.Conversion;

/// <summary>
/// Converts to the <see cref="Version"/> type.
/// </summary>
public sealed class VersionConverter : ValueConverter<Version>
{
    /// <inheritdoc />
    public override Version Convert(string str) => Version.Parse(str);
}