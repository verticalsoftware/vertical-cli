using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Internal;

internal static class Exceptions
{
    public static InvalidOperationException InvalidCommandInvocationPath(ICommand command)
    {
        return new InvalidOperationException($"Command '{command}' is not invokable and has no defined sub-commands");
    }

    public static InvalidOperationException ModelBinderNotDefined(ICommand command)
    {
        return new InvalidOperationException($"Command '{command}' does not have a model binder defined.");
    }

    public static InvalidOperationException InvalidBindingName(string bindingName)
    {
        return new InvalidOperationException($"Binding not found: '{bindingName}'");
    }

    public static InvalidOperationException InvalidBindingName(Type modelType, Type valueType, string bindingName)
    {
        return new InvalidOperationException(
            $"Binding property not configured: IPropertyBinding<{modelType.Name}, {valueType.Name}>.{bindingName}");
    }

    public static InvalidOperationException InvalidBindingCast(Type modelType, Type valueType, IPropertyBinding binding)
    {
        return new InvalidOperationException(
            "Invalid property binding type:" +
            $"\n\texpected: IPropertyBinding<{modelType.Name}, {valueType.Name}>" +
            $"\n\tactual:   IPropertyBinding<{binding.ModelType}, {binding.ValueType}>");
    }

    public static InvalidOperationException ValueConverterNotDefined(IPropertyBinding binding)
    {
        return new InvalidOperationException(
            $"Value converter for type {binding.ValueType} not defined " + 
            $"(required for {binding.ModelType.Name}.{binding.BindingName})");
    }

    public static InvalidOperationException ModelActivatorNotDefined(Type type)
    {
        return new InvalidOperationException(
            $"Model type {type} does not have a parameterless constructor, and an activator was not defined");
    }

    public static InvalidOperationException ConflictingAlias(ISymbolBinding symbol, ISymbol other, string alias)
    {
        return new InvalidOperationException(
            $"Conflicting use of alias '{alias}' between the following symbols:" +
            $"\n\t-> {symbol}" +
            $"\n\t-> {other}");
    }
}