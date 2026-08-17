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
    /// <returns><see cref="Converter{TInput,TOutput}"/></returns>
    Converter<IEnumerable<TElement>, TCollection> GetCollectionConverter<TElement, TCollection>()
        where TCollection : IEnumerable<TElement>;

    /// <summary>
    /// Tries to convert a string argument to <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="symbol">The symbol associated with the value.</param>
    /// <param name="argumentValue">The argument value to convert.</param>
    /// <param name="errorList">The error list.</param>
    /// <typeparam name="TValue">Value type to convert to.</typeparam>
    /// <returns><see cref="ConversionResult{TValue}"/></returns>
    /// <remarks>
    /// This method resolves the argument converter, then performs the conversion is
    /// a guard block. If the conversion fails, an error is added to the provided list.
    /// The result contains the converted value or the default, and the error that may
    /// have occurred.
    /// </remarks>
    ConversionResult<TValue> TryConvertArgument<TValue>(
        ICliSymbol symbol,
        string argumentValue,
        List<CommandLineError> errorList);

    /// <summary>
    /// Accumulates conversions of the given string arguments to <typeparamref name="TElement"/>,
    /// then creates a collection.
    /// </summary>
    /// <param name="symbol">The symbol associated with the value.</param>
    /// <param name="argumentValues">Zero or more string arguments to convert.</param>
    /// <param name="errorList">The error list.</param>
    /// <typeparam name="TElement">The scalar element type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <returns><see cref="ConversionResult{TValue}"/></returns>
    /// <remarks>
    /// This method resolves the argument and collection converters, then performs all argument
    /// conversions in a guard block. The converted values or conversion errors are accumulated,
    /// then included in a result. If all conversions succeed, the result contains a collection,
    /// otherwise the error is an <see cref="AggregateCommandLineError"/>.
    /// </remarks>
    ConversionResult<TCollection> TryConvertCollection<TElement, TCollection>(
        ICliSymbol symbol,
        IEnumerable<string> argumentValues,
        List<CommandLineError> errorList)
        where TCollection : IEnumerable<TElement>;
}