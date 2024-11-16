namespace Vertical.Cli.Conversion;

/// <summary>
/// Internally wraps a list of converters around a dictionary. 
/// </summary>
/// <param name="converters"></param>
public sealed class ValueConverterMap(IList<ValueConverter> converters)
{
    private readonly HashSet<Type> _types = [..converters.Select(cv => cv.TargetType)];

    /// <summary>
    /// Tries to add a value converter.
    /// </summary>
    /// <param name="converter">Converter instance.</param>
    public void TryAdd(ValueConverter converter)
    {
        if (!_types.Add(converter.TargetType))
            return;
        
        converters.Add(converter);
    }

    /// <inheritdoc />
    public override string ToString() => $"{_types.Count}";
}