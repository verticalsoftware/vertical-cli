using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public sealed class BindingInfo
{
    private static readonly HashSet<string> SupportedCollectionTypes = [
        "IEnumerable",
        "ICollection",
        "IReadOnlyCollection",
        "IList",
        "IReadOnlyList",
        "List",
        "LinkedList",
        "ISet",
        "IReadOnlySet",
        "HashSet",
        "SortedSet",
        "Stack",
        "Queue",
        "ImmutableArray",
        "ImmutableList",
        "ImmutableHashSet",
        "ImmutableSortedSet",
        "ImmutableStack",
        "ImmutableQueue"
    ];


    private const string ConversionNs = "global::Vertical.Cli.Conversion";

    private BindingInfo(
        string name,
        string? converterExpression,
        string bindingExpression)
    {
        Name = name;
        ConverterExpression = converterExpression;
        BindingExpression = bindingExpression;
    }

    public string Name { get; }

    public string? ConverterExpression { get; }
    
    public string BindingExpression { get; }

    public static BindingInfo Create(string name, ITypeSymbol symbol)
    {
        return TryCreateCollectionVariant(name, symbol)
               ?? TryCreateArrayVariant(name, symbol)
               ?? CreateSimpleTypeVariant(name, symbol);
    }

    private static BindingInfo CreateSimpleTypeVariant(string name, ITypeSymbol symbol)
    {
        return new BindingInfo(name, GetConverterExpression(symbol),
            $"GetValue<{symbol.ToFqn()}>(\"{name}\")");
    }

    private static BindingInfo? TryCreateArrayVariant(string name, ITypeSymbol symbol)
    {
        return symbol is IArrayTypeSymbol array
            ? new BindingInfo(name,
                GetConverterExpression(array.ElementType),
                $"GetArray<{array.ElementType.ToFqn()}>(\"{name}\")")
            : null;
    }

    private static BindingInfo? TryCreateCollectionVariant(string name, ITypeSymbol symbol)
    {
        if (symbol is not INamedTypeSymbol
            {
                ContainingNamespace:
                {
                    Name: "Generic" or "Immutable",
                    ContainingNamespace:
                    {
                        Name: "Collections",
                        ContainingNamespace.Name: "System"
                    }
                },
                TypeParameters.Length: 1
            } collectionType || !SupportedCollectionTypes.Contains(collectionType.Name))
            return null;

        var valueType = collectionType.TypeArguments[0];
        var bindingExpression = (collectionType.Name switch
        {
            "List" => "GetList<$T>($binding)",
            "LinkedList" => "GetLinkedList<$T>($binding)",
            "IList" => "GetList<$T>($binding)",
            "ICollection" => "GetList<$T>($binding)",
            "IReadOnlyList" => "GetArray<$T>($binding)",
            "IReadOnlyCollection" => "GetArray<$T>($binding)",
            "IEnumerable" => "GetArray<$T>($binding)",
            "HashSet" => "GetHashSet<$T>($binding)",
            "SortedSet" => "GetSortedSet<$T>($binding)",
            "ISet" => "GetHashSet<$T>($binding)",
            "IReadOnlySet" => "GetHashSet<$T>($binding)",
            "Stack" => "GetStack<$T>($binding)",
            "Queue" => "GetQueue<$T>($binding)",
            "ImmutableArray" => "GetImmutableArray<$T>($binding)",
            "ImmutableList" => "GetImmutableList<$T>($binding)",
            "ImmutableHashSet" => "GetImmutableHashSet<$T>($binding)",
            "ImmutableSortedSet" => "GetImmutableHashSet<$T>($binding)",
            "ImmutableStack" => "GetImmutableStack<$T>($binding)",
            "ImmutableQueue" => "GetImmutableQueue<$T>($binding)",
            _ => throw new NotSupportedException()
        }).Replace("$T", valueType.ToFqn()).Replace("$binding", $"\"{name}\"");

        return new BindingInfo(name, GetConverterExpression(valueType), bindingExpression);
    }

    private static string? GetConverterExpression(ITypeSymbol valueType)
    {
        var isParsable = IsParsable(valueType);

        return valueType switch
        {
            { Name: "String", ContainingNamespace.Name: "System" } => $"new {ConversionNs}.StringConverter()",
            
            INamedTypeSymbol { Name: "Nullable", ContainingNamespace.Name: "System", TypeArguments: 
                [{ BaseType: { Name: "Enum", ContainingNamespace.Name: "System" } }] } nullableEnum =>
                $"new {ConversionNs}.NullableEnumConverter<{nullableEnum.TypeArguments[0].ToFqn()}>()",

            { BaseType: { Name: "Enum", ContainingNamespace.Name: "System" } } =>
                $"new {ConversionNs}.EnumConverter<{valueType}>()",
            
            INamedTypeSymbol { Name: "Nullable", ContainingNamespace.Name: "System" } nullableParsable when 
                IsParsable(nullableParsable.TypeArguments[0]) =>
                $"new {ConversionNs}.NullableParsableConverter<{nullableParsable.TypeArguments[0].ToFqn()}>()",
            
            not null when isParsable => $"new {ConversionNs}.ParsableConverter<{valueType.ToFqn()}>()",
            
            { Name: "Uri", ContainingNamespace.Name: "System" } => $"new {ConversionNs}.UriConverter()",
            
            { Name: "DirectoryInfo", ContainingNamespace: { Name: "IO", ContainingNamespace.Name: "System" }} =>
                $"new {ConversionNs}.DirectoryInfoConverter()",
            
            { Name: "FileInfo", ContainingNamespace: { Name: "IO", ContainingNamespace.Name: "System" }} =>
                $"new {ConversionNs}.FileInfoConverter()",
            
            { Name: "Version", ContainingNamespace.Name: "System" } => $"new {ConversionNs}.VersionConverter()",
            
            _ => null
        };
    }

    private static bool IsParsable(ITypeSymbol type)
    {
        return type.Interfaces.Any(t => t is
        {
            Name: "IParsable",
            ContainingNamespace.Name: "System"
        });
    }
}

public static class TypeExtensions
{
    public static string ToFqn(this ITypeSymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}