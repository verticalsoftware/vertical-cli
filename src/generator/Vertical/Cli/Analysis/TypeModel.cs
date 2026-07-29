using Microsoft.CodeAnalysis;

namespace Vertical.Cli.Analysis;

public class TypeModel(ITypeSymbol typeSymbol)
{
    public string GeneratedTypeName { get; } = $"{typeSymbol.Name}Impl_{GetTinyId()}";

    public ITypeSymbol TypeSymbol => typeSymbol;

    private static string GetTinyId() => Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

    public IPropertySymbol[] PropertySymbols { get; } = typeSymbol
        .AllInterfaces
        .Append(typeSymbol)
        .SelectMany(type => type.GetMembers())
        .OfType<IPropertySymbol>()
        .Distinct(SymbolEqualityComparer.Default)
        .Cast<IPropertySymbol>()
        .ToArray();

    public IEnumerable<ITypeSymbol> PropertyTypes => PropertySymbols
        .Select(symbol => symbol.Type)
        .Distinct(SymbolEqualityComparer.Default)
        .Cast<ITypeSymbol>();
}