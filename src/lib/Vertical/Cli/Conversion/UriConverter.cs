namespace Vertical.Cli.Conversion;

/// <summary>
/// Converts arguments to <see cref="Uri"/> instances.
/// </summary>
public sealed class UriConverter : ValueConverter<Uri>
{
    /// <inheritdoc />
    public override Uri Convert(string str) => new(str);
}