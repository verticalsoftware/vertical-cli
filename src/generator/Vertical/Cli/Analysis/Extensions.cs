using Microsoft.CodeAnalysis;

namespace Vertical.Cli.Analysis;

public static class Extensions
{
    private static readonly SymbolDisplayFormat FullyQualifiedNullableFormat =
        new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
    
    extension(ITypeSymbol typeSymbol)
    {
        public bool IsRecordOrInterface => typeSymbol.IsRecord || typeSymbol.TypeKind == TypeKind.Interface;

        public string FullMetadataName => typeSymbol.ContainingNamespace is not null
            ? $"{typeSymbol.ContainingNamespace.ToDisplayString()}.{typeSymbol.MetadataName}"
            : typeSymbol.MetadataName;
        
        public string GlobalName => typeSymbol.ToDisplayString(FullyQualifiedNullableFormat);

        public bool IsParsableType => typeSymbol
            .AllInterfaces
            .Any(interfaceSymbol => interfaceSymbol is
            {
                OriginalDefinition.FullMetadataName: "System.IParsable`1"
            });

        public bool IsNullableType => typeSymbol is
        {
            OriginalDefinition.FullMetadataName: "System.Nullable`1"
        };
        
        public bool IsNullableTypeWithParsableTypeArgument => typeSymbol is
        {
            IsNullableType: true,
            SingleTypeArgument.IsParsableType: true
        };

        public bool IsEnumerableType => typeSymbol
            .AllInterfaces
            .Any(interfaceSymbol => interfaceSymbol is
            {
                OriginalDefinition.FullMetadataName: "System.Collections.Generic.IEnumerable`1"
            });

        public ITypeSymbol? SingleTypeArgument => typeSymbol is INamedTypeSymbol
            {
                TypeArguments.Length: 1
            }
            namedTypeSymbol
            ? namedTypeSymbol.TypeArguments[0]
            : null;
    }
}