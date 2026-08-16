using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration.Assertion.Types;
using Vertical.Cli.Middleware;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class MissingConverterAssertionBuilder : IAssertionBuilder
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
                .Cast<MiddlewareSymbol>()
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