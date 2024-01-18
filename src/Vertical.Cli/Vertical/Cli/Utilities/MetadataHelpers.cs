using System.Reflection;
using Vertical.Cli.Binding;

namespace Vertical.Cli.Utilities;

internal static class MetadataHelpers
{
    internal static Type? GetGenericCollectionType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (!type.IsGenericType)
            return null;

        var genericArguments = type.GetGenericArguments();
        
        // Could be Dictionary, KeyValuePair, etc..
        if (genericArguments.Length != 1)
            return null;

        var genericDefinition = type.GetGenericTypeDefinition();
        
        return !SupportedCollectionTypes.Contains(genericDefinition) 
            ? null 
            : genericArguments[0];
    }

    internal static bool HasModelBinderAttribute(Type type)
    {
        return type
            .GetCustomAttributes()
            .Any(attribute =>
            {
                var attributeType = attribute.GetType();
                return attributeType.IsGenericType && attributeType.GetGenericTypeDefinition() ==
                    typeof(ModelBinderAttribute<>);
            });
    }
}