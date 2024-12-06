namespace Vertical.Cli.Conversion;

/// <summary>
/// Converts types that implement <see cref="IParsable{TSelf}"/>
/// </summary>
/// <typeparam name="T">Parsable value type.</typeparam>
public sealed class ParsableConverter<T> : ValueConverter<T> where T : IParsable<T>
{
    /// <inheritdoc />
    public override T Convert(string str) => T.Parse(str, provider: null);
}