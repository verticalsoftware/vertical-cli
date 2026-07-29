using Microsoft.CodeAnalysis;

namespace Vertical.Cli.Analysis;

public sealed class ConversionInfo
{
    private static readonly ConversionInfo Null = new(null, null);
    private const string ConverterClass = NamingConvention.ConvertersCallFqName;
    private const string EnumerableClass = "global::System.Linq.Enumerable";

    private ConversionInfo(string? argumentConverterCall, string? collectionConverterCall = null)
    {
        ArgumentConverterCall = argumentConverterCall;
        CollectionConverterCall = collectionConverterCall;
    }

    public string? ArgumentConverterCall { get; }
    
    public string? CollectionConverterCall { get; }

    public static ConversionInfo Create(ITypeSymbol type)
    {
        if (GetScalarTypeConverterCall(type) is { } scalarCall)
        {
            return new ConversionInfo(scalarCall);
        }

        if ((type as IArrayTypeSymbol)?.ElementType is { } arrayElementType)
        {
            return new ConversionInfo(
                GetScalarTypeConverterCall(arrayElementType),
                $"({MakeEnumerableTypeName(arrayElementType)} values) => {EnumerableClass}.ToArray(values)");
        }

        if (type is not INamedTypeSymbol { TypeArguments.Length: 1 } namedTypeSymbol)
            return Null;

        var typeArgument = namedTypeSymbol.TypeArguments[0];
        var genericName = namedTypeSymbol.OriginalDefinition.FullMetadataName;
        var conversionFunction = genericName switch
        {
            "System.Collections.Generic.IEnumerable`1" => $"({MakeEnumerableTypeName(typeArgument)} values) => {EnumerableClass}.ToArray(values)",
            "System.Collections.Generic.IReadOnlyCollection`1" => $"({MakeEnumerableTypeName(typeArgument)} values) => {EnumerableClass}.ToArray(values)",
            "System.Collections.Generic.IReadOnlyList`1" => $"({MakeEnumerableTypeName(typeArgument)} values) => {EnumerableClass}.ToArray(values)",
            _ => null
        };

        return conversionFunction is not null
            ? new ConversionInfo(GetScalarTypeConverterCall(typeArgument), conversionFunction)
            : Null;
    }

    private static string? GetScalarTypeConverterCall(ITypeSymbol type)
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

    private static string MakeEnumerableTypeName(ITypeSymbol innerType)
    {
        return $"global::System.Collections.Generic.IEnumerable<{innerType.GlobalName}>";
    }
}