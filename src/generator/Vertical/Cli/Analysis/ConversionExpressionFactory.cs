using Microsoft.CodeAnalysis;

namespace Vertical.Cli.Analysis;

public static class ConversionExpressionFactory
{
    private const string ConverterClass = NamingConvention.ConvertersCallFqName;
    private const string GenericCollections = "global::System.Collections.Generic";
    private const string EnumerableClass = "global::System.Linq.Enumerable";
    
    public static IConversionMethodCall? TryCreate(ITypeSymbol typeSymbol)
    {
        if (GetScalarTypeExpression(typeSymbol) is { } expression)
        {
            return new ArgumentConversionCall(typeSymbol, expression);
        }

        if ((typeSymbol as IArrayTypeSymbol)?.ElementType is { } arrayElementType)
        {
            return new CollectionConversionCall(
                GetArgumentConversionCall(arrayElementType),
                arrayElementType,
                typeSymbol,
                $"values => {EnumerableClass}.ToArray(values)");
        }

        if (typeSymbol is not INamedTypeSymbol { TypeArguments.Length: 1 } namedTypeSymbol)
            return null;

        var typeArgument = namedTypeSymbol.TypeArguments[0];
        var genericName = namedTypeSymbol.OriginalDefinition.FullMetadataName;
        var creationExpression = genericName switch
        {
            "System.Collections.Generic.IEnumerable`1" => $"values => {EnumerableClass}.ToArray(values)",
            "System.Collections.Generic.IReadOnlyCollection`1" => $"values => {EnumerableClass}.ToArray(values)",
            "System.Collections.Generic.IReadOnlyList`1" => $"values => {EnumerableClass}.ToArray(values)",
            "System.Collections.Generic.ICollection`1" => $"values => {EnumerableClass}.ToList(values)",
            "System.Collections.Generic.IList`1" => $"values => {EnumerableClass}.ToList(values)",
            "System.Collections.Generic.List`1" => $"values => {EnumerableClass}.ToList(values)",
            "System.Collections.Generic.Stack`1" => $"values => new {GenericCollections}.Stack<{typeArgument.GlobalName}>(values)",
            "System.Collections.Generic.Queue`1" => $"values => new {GenericCollections}.Queue<{typeArgument.GlobalName}>(values)",
            "System.Collections.Generic.ISet`1" => $"values => new {GenericCollections}.HashSet<{typeArgument.GlobalName}>(values)",
            "System.Collections.Generic.IReadOnlySet`1" => $"values => new {GenericCollections}.HashSet<{typeArgument.GlobalName}>(values)",
            "System.Collections.Generic.HashSet`1" => $"values => new {GenericCollections}.HashSet<{typeArgument.GlobalName}>(values)",
            "System.Collections.Generic.SortedSet`1" => $"values => new {GenericCollections}.SortedSet<{typeArgument.GlobalName}>(values)",
            "System.Collections.Immutable.ImmutableArray`1" => "values => [..values]",
            "System.Collections.Immutable.ImmutableList`1" => "values => [..values]",
            "System.Collections.Immutable.ImmutableHashSet`1" => "values => [..values]",
            "System.Collections.Immutable.ImmutableSortedSet`1" => "values => [..values]",
            "System.Collections.Immutable.ImmutableStack`1" => "values => [..values]",
            "System.Collections.Immutable.ImmutableQueue`1" => "values => [..values]",
            _ => null
        };

        return creationExpression is null
            ? null
            : new CollectionConversionCall(
                GetArgumentConversionCall(typeArgument),
                typeArgument,
                typeSymbol,
                creationExpression);
    }

    private static ArgumentConversionCall? GetArgumentConversionCall(ITypeSymbol typeSymbol)
    {
        return GetScalarTypeExpression(typeSymbol) is { } expression
            ? new ArgumentConversionCall(typeSymbol, expression)
            : null;
    }
    
    private static string? GetScalarTypeExpression(ITypeSymbol type)
    {
        switch (type)
        {
            case { SpecialType: SpecialType.System_String }:
                return $"{ConverterClass}.Default";
            
            case { TypeKind: TypeKind.Enum }:
                return $"{ConverterClass}.Enum<{type.GlobalName}>()";
            
            case INamedTypeSymbol { IsNullableType: true, SingleTypeArgument.TypeKind: TypeKind.Enum } named:
                return $"{ConverterClass}.NullEnum<{named.TypeArguments[0].GlobalName}>()";
            
            case { IsParsableType: true }:
                return $"{ConverterClass}.Parsable<{type.GlobalName}>()";
            
            case INamedTypeSymbol { IsNullableTypeWithParsableTypeArgument: true } nullableType:
                var valueType = nullableType.TypeArguments[0];
                return $"{ConverterClass}.NullParsable<{valueType.GlobalName}>()";
            
            case { FullMetadataName: "System.IO.FileInfo" }:
                return $"{ConverterClass}.FileInfo";
            
            case { FullMetadataName: "System.IO.DirectoryInfo" }:
                return $"{ConverterClass}.DirectoryInfo";
            
            case { FullMetadataName: "System.Uri" }:
                return $"{ConverterClass}.Uri";
            
            default:
                return null;
        }
    }
}