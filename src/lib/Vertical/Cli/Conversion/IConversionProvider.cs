namespace Vertical.Cli.Conversion;

/// <summary>
/// Manages a collection of value converter services.
/// </summary>
public interface IConversionProvider
{
    /// <summary>
    /// Gets an argument converter for the given value type.
    /// </summary>
    /// <typeparam name="TValue">Target value type conversion is requested for.</typeparam>
    /// <returns><see cref="Converter{string, TValue}"/></returns>
    Converter<string, TValue> GetArgumentConverter<TValue>();

    /// <summary>
    /// Gets a converter for the given element and collection type. 
    /// </summary>
    /// <typeparam name="TElement">Element type</typeparam>
    /// <typeparam name="TCollection">Collection type</typeparam>
    /// <returns></returns>
    Converter<IEnumerable<TElement>, TCollection> GetCollectionConverter<TElement, TCollection>()
        where TCollection : IEnumerable<TElement>;
}