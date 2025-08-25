using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator.Utilities;

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

    public static IEnumerable<ITypeSymbol> EnumerateSelfAndBaseTypes(this ITypeSymbol typeSymbol)
    {
        var hashSet = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var stack = new Stack<ITypeSymbol>([typeSymbol]);

        while (stack.Count > 0)
        {
            var next = stack.Pop();
            if (!hashSet.Add(next))
                continue;

            yield return next;

            if (next.BaseType != null)
            {
                stack.Push(next.BaseType);
            }

            foreach (var type in next.Interfaces)
            {
                stack.Push(type);
            }
        }
    }
    
    public static string GetKeywordString(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Internal => "internal",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => "public"
        };
    }

    public static string GetAccessorNotation(this IPropertySymbol propertySymbol)
    {
        return propertySymbol.SetMethod switch
        {
            { IsInitOnly: true } => "get; init;",
            { IsReadOnly: true } => "get;",
            _ => "get; set;"
        };
    }
    
    public static bool IsAssignable(this IPropertySymbol propertySymbol)
    {
        return propertySymbol is
        {
            DeclaredAccessibility: Accessibility.Public or Accessibility.Internal,
            IsReadOnly: false
        };
    }

    public static bool IsNewTypeConstraintClass(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is
        {
            TypeKind: TypeKind.Class,
            IsAbstract: false,
            IsRecord: false
        } && typeSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Count(method => method is
            {
                MethodKind: MethodKind.Constructor,
                Parameters.Length: > 0
            }) == 0;
    }

    public static bool RequiresActivation(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is
        {
            TypeKind: TypeKind.Class,
            IsAbstract: false,
            IsReadOnly: false
        } && typeSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Count(method => method is
            {
                MethodKind: MethodKind.Constructor,
                Parameters.Length: > 0
            }) > 0;
    }
}