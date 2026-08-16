using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;

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
    /// <returns><see cref="Converter{TInput,TOutput}"/></returns>
    Converter<string, TValue> GetArgumentConverter<TValue>();

    /// <summary>
    /// Gets a converter for the given element and collection type. 
    /// </summary>
    /// <typeparam name="TElement">Element type</typeparam>
    /// <typeparam name="TCollection">Collection type</typeparam>
    /// <returns></returns>
    Converter<IEnumerable<TElement>, TCollection> GetCollectionConverter<TElement, TCollection>()
        where TCollection : IEnumerable<TElement>;

    ConversionResult<TValue> TryConvertArgument<TValue>(
        ICliSymbol symbol,
        string argumentValue,
        List<CommandLineError> errorList);

    ConversionResult<TCollection> TryConvertCollection<TElement, TCollection>(
        ICliSymbol symbol,
        IEnumerable<string> argumentValues,
        List<CommandLineError> errorList)
        where TCollection : IEnumerable<TElement>;
}