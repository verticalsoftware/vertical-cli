using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Binding;

public partial class BindingContext
{
    private IBindingSource GetBindingSource(string bindingName)
    {
        if (Parameters.TryGetByBinding(bindingName, out var parameter))
            return parameter;

        if (ProvidedBindings.TryGetValue(bindingName, out var binding))
            return binding;
        
        throw new CliConfigurationException(
            $"Binding {RouteDefinition.ModelType.Name}.{bindingName} is not mapped to any parameter");
    }

    private T SelectSingleValue<T>(string bindingName, IEnumerable<ArgumentSyntax> arguments)
    {
        var bindingSource = GetBindingSource(bindingName);

        if (bindingSource is CliParameter parameter)
        {
            return SelectSingleParameterValue<T>(parameter, arguments.ToArray());
        }

        return bindingSource.TryGetValue(out var obj)
            ? (T)obj!
            : default!;
    }

    private T SelectSingleParameterValue<T>(CliParameter parameter, ArgumentSyntax[] argumentArray)
    {
        ValidateArity(parameter, argumentArray);

        var values = argumentArray.Select(value => ConvertToTargetType<T>(parameter, value)).ToArray();

        if (values.Length == 1)
        {
            return values[0];
        }

        if (parameter.TryGetValue(out var obj))
        {
            return (T)obj!;
        }

        return default!;
    }

    private IEnumerable<T> SelectCollectionValues<T>(string bindingName, IEnumerable<ArgumentSyntax> arguments)
    {
        var bindingSource = GetBindingSource(bindingName);

        if (bindingSource is CliParameter parameter)
        {
            return SelectCollectionParameterValues<T>(parameter, arguments.ToArray());
        }

        return bindingSource.TryGetValue(out var obj) && obj != null
            ? CastToEnumerable<T>(obj)
            : [];
    }

    private IEnumerable<T> SelectCollectionParameterValues<T>(CliParameter parameter, ArgumentSyntax[] argumentArray)
    {
        ValidateArity(parameter, argumentArray);

        var values = argumentArray.Select(value => ConvertToTargetType<T>(parameter, value)).ToArray();

        if (values.Length > 0)
        {
            return values;
        }

        return parameter.TryGetValue(out var obj)
            ? CastToEnumerable<T>(obj)
            : [];
    }

    private static IEnumerable<T> CastToEnumerable<T>(object? obj)
    {
        return obj switch
        {
            IEnumerable<T> enumerable => enumerable,
            null => [],
            _ => throw new InvalidCastException($"Could not convert {obj} to {typeof(IEnumerable<T>)}")
        };
    }

    private T ConvertToTargetType<T>(CliParameter parameter, ArgumentSyntax argument)
    {
        var value = ResolveArgumentValue(parameter, argument);
        
        if (!ValueConverters.TryGetValue(typeof(T), out var converter))
        {
            throw Exceptions.ConverterNotFound(typeof(T), parameter);
        }

        var typeConverter = (ValueConverter<T>)converter;

        try
        {
            return typeConverter.Convert(value);
        }
        catch(Exception exception)
        {
            throw Exceptions.ConversionFailed(RouteDefinition, parameter, argument, converter, exception);
        }
    }

    private static string ResolveArgumentValue(CliParameter parameter, ArgumentSyntax argument)
    {
        return (parameter.SymbolKind, argument) switch
        {
            { SymbolKind: SymbolKind.Switch } => bool.TrueString,
            { argument: { IdentifierSymbol.Length: > 0, OperandValue.Length: > 0 }} when parameter.Identifiers.Any(id =>
                id.IdentifierSymbol == argument.IdentifierSymbol) => argument.OperandValue,
            _ => argument.Text
        };
    }

    private void ValidateArity(CliParameter parameter, ArgumentSyntax[] arguments)
    {
        var (min, max) = (parameter.Arity.MinCount, parameter.Arity.MaxCount);
        var count = arguments.Length;

        if (count < min)
        {
            throw Exceptions.InvalidMinimumArity(RouteDefinition, parameter, arguments);
        }

        if (max == null || count <= max)
            return;

        throw Exceptions.InvalidMaximumArity(RouteDefinition, parameter, arguments);
    }
}