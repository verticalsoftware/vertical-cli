using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Binding;

/// <summary>
/// Extends the <see cref="PropertyBindingInfo"/> type.
/// </summary>
public static class BindingExtensions
{
    extension(PropertyBindingInfo bindingInfo)
    {
        /// <summary>
        /// Creates a single value binding result.
        /// </summary>
        /// <param name="symbol">The bound symbol.</param>
        /// <param name="argument">The string argument value to convert.</param>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <returns><see cref="BindingResult{TValue}"/></returns>
        public BindingResult<TValue> CreateScalarBindingResult<TModel, TValue>(
            CliSymbol<TModel, TValue> symbol,
            string argument) 
            where TModel : class
        {
            var converter = bindingInfo.ConversionProvider.GetArgumentConverter<TValue>();

            try
            {
                var value = converter(argument);
                return new BindingResult<TValue>(symbol.BindingName, value);
            }
            catch (Exception exception)
            {
                var error = ArgumentConversionError.Create(
                    symbol, 
                    typeof(TValue), 
                    argument,
                    bindingInfo.HelpProvider,
                    exception);
                
                return new BindingResult<TValue>(symbol.BindingName, default!, error);
            }
        }

        /// <summary>
        /// Creates a collection value result.
        /// </summary>
        /// <param name="symbol">The bound symbol.</param>
        /// <param name="arguments">The arguments to convert and place in the collection.</param>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TElement">Element type</typeparam>
        /// <typeparam name="TCollection">Collection type</typeparam>
        /// <returns><see cref="BindingResult{TValue}"/></returns>
        public BindingResult<TCollection> CreateCollectionBindingResult<TModel, TElement, TCollection>(
            CliSymbol<TModel, TCollection> symbol,
            IEnumerable<string> arguments)
            where TModel : class
            where TCollection : IEnumerable<TElement>
        {
            var conversionProvider = bindingInfo.ConversionProvider;
            var argumentConverter = conversionProvider.GetArgumentConverter<TElement>();
            var collectionConverter = conversionProvider.GetCollectionConverter<TElement, TCollection>();
            var argumentArray = arguments.ToArray();
            var valueList = new List<TElement>(argumentArray.Length);
            List<CommandLineError>? errors = null;

            foreach (var argument in argumentArray)
            {
                try
                {
                    valueList.Add(argumentConverter(argument));
                }
                catch (Exception exception)
                {
                    var error = ArgumentConversionError.Create(
                        symbol, 
                        typeof(TElement), 
                        argument,
                        bindingInfo.HelpProvider,
                        exception);
                    
                    (errors ??= []).Add(error);
                }
            }

            if (errors?.Count > 0)
            {
                return new BindingResult<TCollection>(
                    symbol.BindingName,
                    default!,
                    new AggregateCommandLineError(errors));
            }

            var collection = collectionConverter(valueList);
            return new BindingResult<TCollection>(symbol.BindingName, collection);
        }
    }

    /// <summary>
    /// Gets zero or more errors contained in the binding results.
    /// </summary>
    /// <param name="bindingResults">The binding result dictionary.</param>
    /// <returns>An enumeration that contains detected input errors.</returns>
    public static IEnumerable<CommandLineError> GetErrors(this Dictionary<string, IBindingResult> bindingResults)
    {
         return  bindingResults
            .Select(keyValue => keyValue.Value.Error)
            .Where(error => error is not null)
            .Cast<CommandLineError>();
    }
}