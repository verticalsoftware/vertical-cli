using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vertical.Cli.SourceGenerator;

public static class Utilities
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
    
    public static string GetSimpleTypeName(this TypeSyntax typeSyntax)
    {
        return typeSyntax switch
        {
            SimpleNameSyntax simple => simple.Identifier.Text,
            QualifiedNameSyntax qualified => qualified.Right.GetSimpleTypeName(),
            AliasQualifiedNameSyntax alias => alias.Alias.GetSimpleTypeName(),
            _ => string.Empty
        };
    }

    public static string Format(this ITypeSymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    
    public static bool IsAsyncResultType(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is INamedTypeSymbol
        {
            Name: "Task",
            ContainingNamespace:
            {
                Name: "Tasks",
                ContainingNamespace:
                {
                    Name: "Threading",
                    ContainingNamespace.Name: "System"
                }
            }
        };
    }

    public static bool ResultTypeHasValue(this ITypeSymbol typeSymbol)
    {
        return typeSymbol switch
        {
            INamedTypeSymbol {
                Name: "Task",
                ContainingNamespace:
                {
                    Name: "Tasks",
                    ContainingNamespace:
                    {
                        Name: "Threading",
                        ContainingNamespace.Name: "System"
                    }
                },
                TypeArguments.Length: 0
            } => false,
            {
                Name: "Void",
                ContainingNamespace.Name: "System"
            } => false,
            _ => true
        };
    }

    public static bool TryGetCompatibleConstructor(this INamedTypeSymbol symbol, out IMethodSymbol? methodSymbol)
    {
        methodSymbol = symbol
            .Constructors
            .FirstOrDefault(ctor => ctor.IsCompatibleModelConstructor());

        return methodSymbol != null;
    }

    public static bool ImplementsIParsableInterface(this ITypeSymbol symbol)
    {
        return symbol.Interfaces.Any(type => type is
        {
            Name: "IParsable",
            ContainingNamespace.Name: "System"
        });
    }

    public static bool IsNullableValueType(this ITypeSymbol symbol)
    {
        return symbol is
        {
            Name: "Nullable",
            ContainingNamespace.Name: "System"
        };
    }

    public static bool IsEnum(this ITypeSymbol symbol)
    {
        return symbol is
        {
            BaseType:
            {
                Name: "Enum",
                ContainingNamespace.Name: "System"
            }
        };
    }

    public static IEnumerable<IPropertySymbol> GetAllProperties(this INamedTypeSymbol symbol)
    {
        var target = symbol;

        for (; target != null; target = target.BaseType)
        {
            var properties = target
                .GetMembers()
                .Where(member => member is IPropertySymbol { IsReadOnly: false })
                .Cast<IPropertySymbol>();

            foreach (var property in properties)
            {
                yield return property;
            }
        }
    }

    public static bool IsSupportedCollectionType(this ITypeSymbol type) => type is
    {
        ContainingNamespace:
        {
            Name: "Generic" or "Immutable",
            ContainingNamespace:
            {
                Name: "Collections",
                ContainingNamespace.Name: "System"
            }
        }
    } && SupportedCollectionTypes.Contains(type.Name);
    
    public static bool TryGetSupportedCollectionArgumentType(this INamedTypeSymbol symbol, out ITypeSymbol? elementType)
    {
        elementType = null;

        if (symbol is not
            {
                ContainingNamespace:
                {
                    Name: "Generic" or "Immutable",
                    ContainingNamespace:
                    {
                        Name: "Collections",
                        ContainingNamespace.Name: "System"
                    }
                }
            })
            return false;

        if (SupportedCollectionTypes.Contains(symbol.Name))
        {
            elementType = symbol.TypeArguments[0];
            return true;
        }

        return false;
    }

    private static bool IsCompatibleModelConstructor(this IMethodSymbol symbol)
    {
        return symbol.DeclaredAccessibility == Accessibility.Public
               && (symbol.Parameters.Length != 1 || 
                   !symbol.Parameters[0].Type.Equals(symbol.ContainingSymbol, SymbolEqualityComparer.Default));
    }
}