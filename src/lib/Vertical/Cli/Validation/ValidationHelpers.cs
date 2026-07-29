using Vertical.Cli.Configuration;

namespace Vertical.Cli.Validation;

internal static class ValidationHelpers
{
    public static Action<CliSymbol, ValidationContext>? TryCreateValidationAction<TModel, TValue>(
        Action<ValidationEventInfo<TModel, TValue>>? validate) 
        where TModel : class
    {
        return validate is null
            ? null
            : (symbol, context) =>
            {
                var validationInfo = new ValidationEventInfo<TModel, TValue>(
                    context,
                    symbol,
                    (TModel)context.Model,
                    ((CliSymbol<TModel, TValue>)symbol).GetValue(context.Model));

                validate(validationInfo);
            };
    }

    public static Action<CliSymbol, ValidationContext>? TryCreateValidationAction<TModel, TElement, TCollection>(
        Action<ValidationEventInfo<TModel, TElement, TCollection>>? validate)
        where TModel : class
        where TCollection : IEnumerable<TElement>
    {
        return validate is null
            ? null
            : (symbol, context) =>
            {
                var namedTypeSymbol = (CliSymbol<TModel, TCollection>)symbol;
                var validationInfo = new ValidationEventInfo<TModel, TElement, TCollection>(
                    context,
                    namedTypeSymbol,
                    (TModel)context.Model,
                    namedTypeSymbol.GetValue(context.Model));

                validate(validationInfo);
            };
    }
}