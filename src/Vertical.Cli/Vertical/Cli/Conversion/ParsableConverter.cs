namespace Vertical.Cli.Conversion;

/// <summary>
/// Leverages the <see cref="IParsable{TSelf}"/> interface for conversion.
/// </summary>
/// <typeparam name="T">Value type</typeparam>
public sealed class ParsableConverter<T> : ValueConverter<T> where T : IParsable<T>
{
    /// <inheritdoc />
    public override T Convert(string s)
    {
        return T.Parse(s, provider: null);
    }
}

/// <summary>
/// Leverages the <see cref="IParsable{TSelf}"/> interface for conversion.
/// </summary>
/// <typeparam name="T">Value type</typeparam>
public sealed class NullableParsableConverter<T> : ValueConverter<T?> where T : struct, IParsable<T>
{
    /// <inheritdoc />
    public override T? Convert(string s)
    {
        return T.Parse(s, provider: null);
    }
}