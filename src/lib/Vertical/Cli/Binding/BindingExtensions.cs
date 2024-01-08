using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Binding;

internal static class BindingExtensions
{
    private static readonly Reusable<List<string>> ReusableErrorList = new(
        () => new List<string>(),
        list => list.Clear());

    internal static T GetBindingValue<T>(
        this IBindingPath bindingPath,
        SymbolDefinition<T> symbol,
        string value)
        where T : notnull
    {
        var convertedValue = bindingPath.ConvertValue(symbol, value);

        return bindingPath.ValidateValue(symbol, convertedValue);
    }

    internal static T ConvertValue<T>(
        this IBindingPath bindingPath,
        SymbolDefinition<T> symbol,
        string value)
        where T : notnull
    {
        var converter = symbol.Converter
                        ?? bindingPath.ConverterDictionary.GetValueOrDefault(typeof(T))
                        ?? DefaultConverter<T>.Value
                        ?? throw new InvalidOperationException();

        try
        {
            return ((ValueConverter<T>)converter).Convert(new ConversionContext<T>(symbol, value));
        }
        catch (Exception exception)
        {
            throw InvocationExceptions.ValueConversionFailed(symbol, exception, value);
        }
    }

    internal static T ValidateValue<T>(
        this IBindingPath bindingPath,
        SymbolDefinition<T> symbol,
        T value)
        where T : notnull
    {
        var validator = symbol.Validator ?? bindingPath.ValidatorDictionary.GetValueOrDefault(typeof(T));

        if (validator == null)
            return value;

        var errorList = ReusableErrorList.GetInstance();

        try
        {
            ((Validator<T>)validator).Validate(new ValidationContext<T>(symbol, value, errorList));

            if (errorList.Count == 0)
                return value;

            throw InvocationExceptions.ValidationFailed(symbol, value, errorList.ToArray());
        }
        catch (Exception exception)
        {
            throw InvocationExceptions.ValidationFailed(symbol, value, exception);
        }
        finally
        {
            ReusableErrorList.Return(errorList);
        }
    }
}