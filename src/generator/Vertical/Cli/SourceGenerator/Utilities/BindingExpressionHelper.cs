using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator.Utilities;

public class BindingExpressionHelper(string contextParameter)
{
    private readonly ConcurrentDictionary<ITypeSymbol, string> _cachedScalarConverters = new(
        SymbolEqualityComparer.IncludeNullability);

    private readonly Dictionary<string, string> _collectionConversionExpressions = new()
    {
        ["Array"] = "values => [..values]",
        
        // Concrete types
        ["List"] = "values => [..values]",
        ["LinkedList"] = "values => new System.Collections.Generic.LinkedList<{0}>(values)",
        ["HashSet"] = "values => new System.Collections.Generic.HashSet<{0}>(values)",
        ["SortedSet"] = "values => new System.Collections.Generic.SortedSet<{0}>(values)",
        ["Stack"] = "values => new System.Collections.Generic.Stack<{0}>(values)",
        ["Queue"] = "values => new System.Collections.Generic.Queue<{0}>(values)",
        ["ImmutableArray"] = "values => [..values]",
        ["ImmutableList"] = "values => [..values]",
        ["ImmutableHashSet"] = "values => [..values]",
        ["ImmutableSortedSet"] = "values => [..values]",
        ["ImmutableStack"] = "values => [..values]",
        ["ImmutableQueue"] = "values => [..values]",
        
        // Read-only interfaces
        ["IEnumerable"] = "values => System.Linq.Enumerable.ToArray(values)",
        ["IReadOnlyCollection"] = "values => System.Linq.Enumerable.ToArray(values)",
        ["IReadOnlyList"] = "values => System.Linq.Enumerable.ToArray(values)",
        ["IReadOnlySet"] = "values => new System.Collections.Generic.HashSet<{0}>(values)",
        
        // Writeable interfaces
        ["ICollection"] = "values => System.Linq.Enumerable.ToList(values)",
        ["IList"] = "values => System.Linq.Enumerable.ToList(values)",
        ["ISet"] = "values => new System.Collections.Generic.HashSet<{0}>(values)",
    };

    private const string ConverterQualifiedClassName = "Vertical.Cli.Conversion.Converters";

    public string ContextParameter => contextParameter;

    public string GetBindingExpression(IPropertySymbol propertySymbol)
    {
        return GetBindingExpression(propertySymbol.Type, propertySymbol.Name);
    }

    public string GetBindingExpression(IParameterSymbol parameterSymbol)
    {
        return GetBindingExpression(parameterSymbol.Type, parameterSymbol.Name);
    }

    private string GetBindingExpression(ITypeSymbol typeSymbol, string bindingName)
    {
        return TryGetCollectionValueExpression(typeSymbol, bindingName)
               ?? TryGetArrayValueExpression(typeSymbol, bindingName)
               ?? GetScalarValueExpression(typeSymbol, bindingName);
    }

    private string? TryGetArrayValueExpression(ITypeSymbol typeSymbol, string bindingName)
    {
        return typeSymbol is IArrayTypeSymbol arraySymbol
            ? GetCollectionValueExpression(typeSymbol,
                arraySymbol.ElementType, 
                _collectionConversionExpressions["Array"],
                bindingName)
            : null;
    }

    private string GetScalarValueExpression(ITypeSymbol typeSymbol, string bindingName)
    {
        var conversionExpression = GetConversionExpression(typeSymbol);

        return $"{contextParameter}.GetValue(x => x.{bindingName}, {conversionExpression})";
    }

    private string? TryGetCollectionValueExpression(ITypeSymbol typeSymbol, string bindingName)
    {
        if (!_collectionConversionExpressions.TryGetValue(
                typeSymbol.Name,
                out var expressionTemplate))
        {
            return null;
        }
        
        // Out of box, only support BCL collections.
        if (typeSymbol is not INamedTypeSymbol
            {
                // Exclude associative collection types
                TypeArguments.Length: 1,
                ContainingNamespace:
                {
                    Name: "Generic" or "Immutable",
                    ContainingNamespace:
                    {
                        Name: "Collections",
                        ContainingNamespace.Name: "System"
                    }
                }
            } genericCollectionType)
        {
            return null;
        }

        return GetCollectionValueExpression(
            typeSymbol, 
            genericCollectionType.TypeArguments[0], 
            expressionTemplate,
            bindingName);
    }

    private string GetCollectionValueExpression(
        ITypeSymbol collectionType,
        ITypeSymbol valueType,
        string collectionConversionTemplate,
        string bindingName)
    {
        var elementConversionExpression = GetConversionExpression(valueType);
        var formattedExpression = string.Format(collectionConversionTemplate, valueType);
        var isNullableCollection = collectionType is
        {
            IsReferenceType: true,
            NullableAnnotation: NullableAnnotation.Annotated
        };
        var contextMethod = isNullableCollection
            ? "GetCollectionValueOrDefault"
            : "GetCollectionValue";
        var resolvedCollectionType = isNullableCollection
            ? collectionType.WithNullableAnnotation(NullableAnnotation.NotAnnotated)
            : collectionType;
        var bindingNameAnnotation = isNullableCollection ? "!" : null;

        return elementConversionExpression != "null"
            ? $"{contextParameter}.{contextMethod}<{valueType}, {resolvedCollectionType}>(x => x.{bindingName}{bindingNameAnnotation}, {elementConversionExpression}, {formattedExpression})"
            : $"{contextParameter}.{contextMethod}<{valueType}, {resolvedCollectionType}>(x => x.{bindingName}{bindingNameAnnotation}, null, {formattedExpression})";
    }

    private string GetConversionExpression(ITypeSymbol valueType)
    {
        return _cachedScalarConverters.GetOrAdd(
            valueType,
            ResolveTypeConverterExpression);
    }

    private static string ResolveTypeConverterExpression(ITypeSymbol type)
    {
        switch (type)
        {
            case { SpecialType: SpecialType.System_String }:
                return $"{ConverterQualifiedClassName}.Default";
            
            case { SpecialType: SpecialType.System_Boolean }:
                return $"{ConverterQualifiedClassName}.Boolean";
            
            case INamedTypeSymbol
            {
                Name: "Nullable",
                ContainingNamespace.Name: "System",
                TypeArguments: [{ BaseType.SpecialType: SpecialType.System_Enum }]
            } nullableEnum:
                return $"{ConverterQualifiedClassName}.NullableEnum<{nullableEnum.TypeArguments[0]}>()";
            
            case { BaseType.SpecialType: SpecialType.System_Enum }:
                return $"{ConverterQualifiedClassName}.Enum<{type}>()";
            
            case { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated } when type.IsParsable():
                return $"{ConverterQualifiedClassName}.NullAnnotatedParsable<{type.WithNullableAnnotation(NullableAnnotation.NotAnnotated)}>()";
            
            case not null when type.IsParsable():
                return $"{ConverterQualifiedClassName}.Parsable<{type}>()";
            
            case INamedTypeSymbol nullable when nullable.IsNullableAndParsable():
                return $"{ConverterQualifiedClassName}.Nullable<{nullable.TypeArguments[0]}>()";
            
            case
            {
                Name: "DirectoryInfo",
                ContainingNamespace:
                {
                    Name: "IO",
                    ContainingNamespace.Name: "System"
                }
            }:
                return $"{ConverterQualifiedClassName}.DirectoryInfo";
            
            case
            {
                Name: "FileInfo",
                ContainingNamespace:
                {
                    Name: "IO",
                    ContainingNamespace.Name: "System"
                }
            }:
                return $"{ConverterQualifiedClassName}.FileInfo";
            
            case
            {
                Name: "FileSystemInfo",
                ContainingNamespace:
                {
                    Name: "IO",
                    ContainingNamespace.Name: "System"
                }
            }:
                return $"{ConverterQualifiedClassName}.FileSystemInfo";
            
            case { Name: "Uri", ContainingNamespace.Name: "System" }:
                return $"{ConverterQualifiedClassName}.Uri";
            
            case { Name: "Version", ContainingNamespace.Name: "System" }:
                return $"{ConverterQualifiedClassName}.Version";
            
            default:
                return "null";
        }
    }
}