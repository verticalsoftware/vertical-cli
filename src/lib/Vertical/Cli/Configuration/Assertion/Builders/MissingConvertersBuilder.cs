using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class MissingConvertersBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        var propertyTypes = context
            .CallSites
            .Select(command => context.GetModelConfiguration(command.ModelType!))
            .SelectMany(configuration => configuration
                .BindingSources
                .Select(source => source.ValueType))
            .Concat(context
                .Directives
                .Select(directive => directive.ParameterType)
                .Where(type => type is not null)
                .Cast<Type>())
            .Distinct();

        var configuration = context.Configuration;

        foreach (var propertyType in propertyTypes)
        {
            if (TryGetCollectionType(propertyType, out var elementType))
            {
                if (configuration.HasCollectionConverter(elementType, propertyType))
                    continue;
                
                context.Assertions.Add(new MissingCollectionConverterAssertion(elementType, propertyType));
                continue;
            }

            if (context.Configuration.HasArgumentConverter(propertyType))
                continue;
            
            context.Assertions.Add(new MissingArgumentConverterAssertion(propertyType));
        }
    }

    private static bool HasConverter(AssertionContext context, Type type)
    {
        var provider = context.Configuration;

        return
            (TryGetCollectionType(type, out var elementType) && provider.HasCollectionConverter(elementType, type))
            ||
            provider.HasArgumentConverter(type);
    }

    private static bool TryGetCollectionType(Type type, [NotNullWhen(true)] out Type? elementType)
    {
        elementType = null;

        if (type == typeof(string))
            return false;
        
        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        var enumerableInterface = type
            .GetInterfaces()
            .FirstOrDefault(interfaceType =>
                interfaceType.IsGenericType
                && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (enumerableInterface is null)
            return false;

        elementType = enumerableInterface.GenericTypeArguments[0];
        return true;
    }
}