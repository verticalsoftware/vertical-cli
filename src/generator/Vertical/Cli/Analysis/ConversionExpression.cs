using Microsoft.CodeAnalysis;

namespace Vertical.Cli.Analysis;

public static class ConversionExpression
{
    public sealed class CollectionExpression(
        ITypeSymbol elementType,
        ITypeSymbol collectionType,
        string expression)
    {
        public ITypeSymbol ElementType => elementType;
        public ITypeSymbol CollectionType => collectionType;
        public string Expression => expression;
    }
    
    private const string ConverterClass = NamingConvention.ConvertersCallFqName;
    private const string GenericCollections = "global::System.Collections.Generic";
    private const string GenericCollectionsMetadata = "System.Collections.Generic";
    private const string ImmutableCollectionsMetadata = "System.Collections.Immutable";
    private const string EnumerableClass = "global::System.Linq.Enumerable";
    private const string HashSetClass = $"{GenericCollections}.HashSet";
    private const string SortedSetClass = $"{GenericCollections}.SortedSet";
    private const string StackClass = $"{GenericCollections}.Stack";
    private const string QueueClass = $"{GenericCollections}.Queue";

    private enum EnumerableExtension
    {
        ToArray,
        ToList
    }

    private enum ConversionMemberProperty
    {
        Default,
        FileInfo,
        DirectoryInfo,
        Uri
    }

    private enum ConversionMemberMethod
    {
        Enum,
        NullEnum,
        Parsable,
        NullParsable
    }

    public static string? TryCreateScalarExpression(ITypeSymbol typeSymbol)
    {
        return typeSymbol switch
        {
            { SpecialType: SpecialType.System_String } => GetConverterClassProperty(ConversionMemberProperty.Default),
            { TypeKind: TypeKind.Enum } => GetConverterClassMethod(typeSymbol, ConversionMemberMethod.Enum),
            { IsParsableType: true } => GetConverterClassMethod(typeSymbol, ConversionMemberMethod.Parsable),
            INamedTypeSymbol { IsNullableType: true, SingleTypeArgument.TypeKind: TypeKind.Enum } nullEnum =>
                GetConverterClassMethod(nullEnum.TypeArguments[0], ConversionMemberMethod.NullEnum),
            INamedTypeSymbol { IsNullableTypeWithParsableTypeArgument: true } nullParsable =>
                GetConverterClassMethod(nullParsable.TypeArguments[0], ConversionMemberMethod.NullParsable),
            { FullMetadataName: "System.IO.FileInfo" } => GetConverterClassProperty(ConversionMemberProperty.FileInfo),
            { FullMetadataName: "System.IO.DirectoryInfo" } => GetConverterClassProperty(ConversionMemberProperty.DirectoryInfo),
            { FullMetadataName: "System.Uri" } => GetConverterClassProperty(ConversionMemberProperty.Uri),
            _ => null
        };
    }

    public static CollectionExpression? TryCreateCollectionExpression(ITypeSymbol typeSymbol)
    {
        if ((typeSymbol as IArrayTypeSymbol)?.ElementType is { } arrayElementType)
        {
            return new CollectionExpression(
                arrayElementType,
                typeSymbol,
                CallEnumerableExtension(arrayElementType, EnumerableExtension.ToArray));
        }

        if (typeSymbol is not INamedTypeSymbol { TypeArguments.Length: 1 } namedTypeSymbol)
            return null;

        var typeArgument = namedTypeSymbol.TypeArguments[0];
        var genericName = namedTypeSymbol.OriginalDefinition.FullMetadataName;

        const string spreadOperation = "values => [..values]";

        var expression =  genericName switch
        {
            // Interfaces
            $"{GenericCollectionsMetadata}.IEnumerable`1" => CallEnumerableExtension(typeArgument, EnumerableExtension.ToArray), 
            $"{GenericCollectionsMetadata}.ICollection`1" => CallEnumerableExtension(typeArgument,EnumerableExtension.ToList), 
            $"{GenericCollectionsMetadata}.IReadOnlyCollection`1" => CallEnumerableExtension(typeArgument,EnumerableExtension.ToArray), 
            $"{GenericCollectionsMetadata}.IList`1" => CallEnumerableExtension(typeArgument,EnumerableExtension.ToList), 
            $"{GenericCollectionsMetadata}.IReadOnlyList`1" => CallEnumerableExtension(typeArgument,EnumerableExtension.ToArray), 
            $"{GenericCollectionsMetadata}.ISet`1" => CallCollectionConstructor(typeArgument, HashSetClass), 
            $"{GenericCollectionsMetadata}.IReadOnlySet`1" => CallCollectionConstructor(typeArgument, HashSetClass), 
            
            // Hard types
            $"{GenericCollectionsMetadata}.List`1" => CallEnumerableExtension(typeArgument,EnumerableExtension.ToList),
            $"{GenericCollectionsMetadata}.Stack`1" => CallCollectionConstructor(typeArgument, StackClass),
            $"{GenericCollectionsMetadata}.Queue`1" => CallCollectionConstructor(typeArgument, QueueClass),
            $"{GenericCollectionsMetadata}.HashSet`1" => CallCollectionConstructor(typeArgument, HashSetClass),
            $"{GenericCollectionsMetadata}.SortedSet`1" => CallCollectionConstructor(typeArgument, SortedSetClass),
            
            // Immutable types
            $"{ImmutableCollectionsMetadata}.ImmutableArray`1" => spreadOperation,
            $"{ImmutableCollectionsMetadata}.ImmutableList`1" => spreadOperation,
            $"{ImmutableCollectionsMetadata}.ImmutableHashSet`1" => spreadOperation,
            $"{ImmutableCollectionsMetadata}.ImmutableSortedSet`1" => spreadOperation,
            $"{ImmutableCollectionsMetadata}.ImmutableStack`1" => spreadOperation,
            $"{ImmutableCollectionsMetadata}.ImmutableQueue`1" => spreadOperation,
            
            _ => null
        };

        return expression is not null
            ? new CollectionExpression(typeArgument, typeSymbol, expression)
            : null;
    }

    private static string CallEnumerableExtension(ITypeSymbol typeArgument, EnumerableExtension extension)
    {
        return $"values => {EnumerableClass}.{extension}(values)";
    }

    private static string CallCollectionConstructor(ITypeSymbol typeArgument, string className)
    {
        return $"values => new {className}<{typeArgument.GlobalName}>(values)";
    }

    private static string EnumerableType(ITypeSymbol typeSymbol) => $"{EnumerableClass}<{typeSymbol.GlobalName}>";

    private static string GetConverterClassProperty(ConversionMemberProperty property)
    {
        return $"{ConverterClass}.{property}";
    }

    private static string GetConverterClassMethod(ITypeSymbol typeSymbol, ConversionMemberMethod method)
    {
        return $"{ConverterClass}.{method}<{typeSymbol.GlobalName}>()";
    }
}