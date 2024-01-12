using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Invocation;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Binding;

internal static class BindingExtensions
{
    private static readonly Reusable<List<string>> ReusableErrorList = new(
        () => new List<string>(),
        list => list.Clear());

    internal static T GetBindingValue<T>(
        this IBindingContext bindingContext,
        SymbolDefinition<T> symbol,
        string value)
    {
        var convertedValue = bindingContext.ConvertValue(symbol, value);

        return bindingContext.ValidateValue(symbol, convertedValue);
    }

    private static T ConvertValue<T>(
        this IBindingContext bindingContext,
        SymbolDefinition<T> symbol,
        string value)
    {
        var converter = bindingContext.ConverterDictionary.GetValueOrDefault(typeof(T))
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

    private static T ValidateValue<T>(
        this IBindingContext bindingContext,
        SymbolDefinition<T> symbol,
        T value)
    {
        var validator = symbol.Validator ?? bindingContext.ValidatorDictionary.GetValueOrDefault(typeof(T));

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