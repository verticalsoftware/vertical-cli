namespace Vertical.Cli.Conversion;

/// <summary>
/// Returns the given string.
/// </summary>
public sealed class StringConverter : ValueConverter<string>
{
    /// <inheritdoc />
    public override string Convert(string str) => str;
}