using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public static class TypeSymbolExtensions
{
    public static bool IsParsable(this ITypeSymbol typeSymbol)
    {
        return typeSymbol
            .Interfaces
            .Any(interfaceSymbol => interfaceSymbol is
            {
                Name: "IParsable",
                TypeArguments.Length: 1,
                ContainingNamespace.Name: "System"
            });
    }

    public static bool IsNullableAndParsable(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is INamedTypeSymbol
        {
            Name: "Nullable",
            ContainingNamespace.Name: "System"
        } nullableType && nullableType.TypeArguments[0].IsParsable();
    }

    public static bool IsDeclarable(this IPropertySymbol propertySymbol)
    {
        return propertySymbol.ContainingType.TypeKind == TypeKind.Interface;
    }
    
    public static bool IsAssignable(this IPropertySymbol propertySymbol)
    {
        return propertySymbol is
        {
            DeclaredAccessibility: Accessibility.Public or Accessibility.Internal,
            IsReadOnly: false
        };
    }
}