using System.Text;
using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public static class SymbolExtensions
{
    private static readonly string[] SupportedCollectionTypes =
    {
        "List",
        "IList",
        "IReadOnlyList",
        "ICollection",
        "IReadOnlyCollection",
        "IEnumerable",
        "HashSet",
        "SortedSet",
        "ISet",
        "IReadOnlySet",
        "Stack",
        "Queue"
    };
    
    public static bool TryGetCommandBuilderGenericArguments(
        this IMethodSymbol configureMethodSymbol,
        int builderParameterIndex,
        out ITypeSymbol? modelType,
        out ITypeSymbol? resultType)
    {
        modelType = resultType = null;

        var parameters = configureMethodSymbol.Parameters;
        if (parameters.Length != builderParameterIndex + 1)
            return false;

        var parameterType = parameters[builderParameterIndex].Type;

        if (!(parameterType is INamedTypeSymbol actionType && actionType.GetGenericTypeName() == "System.Action")) 
            return false;

        if (actionType.TypeArguments.Length != 1)
            return false;

        var actionArgumentType = actionType.TypeArguments[0];
        
        if (actionArgumentType.GetGenericTypeName() != "Vertical.Cli.Configuration.ICommandBuilder")
            return false;

        var builderType = (INamedTypeSymbol)actionArgumentType;

        (modelType, resultType) = (builderType.TypeArguments[0], builderType.TypeArguments[1]);

        return true;
    }

    public static bool IsTaskType(this ITypeSymbol symbol) => symbol.GetGenericTypeName() == "System.Threading.Tasks.Task";

    public static bool IsNoneModelType(this ITypeSymbol symbol) => symbol.ToDisplayString() == "Vertical.Cli.None";

    public static bool IsSupportedGenericCollectionType(this ITypeSymbol symbol)
    {
        return symbol.ContainingNamespace.ToDisplayString() == "System.Collections.Generic" &&
               SupportedCollectionTypes.Contains(symbol.Name);
    }
    
    public static string CreateVariableName(this ITypeSymbol symbol)
    {
        var sb = new StringBuilder();
        
        symbol.Name.ForEach((chr, index) =>
        {
            if (!char.IsLetterOrDigit(chr))
                return;

            if (index == 0)
            {
                sb.Append(char.ToLower(chr));
                return;
            }

            sb.Append(chr);
        });

        return sb.ToString();
    }

    public static string ToFullName(this ITypeSymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    public static IReadOnlyCollection<ParameterBinding> GetBindings(this ITypeSymbol symbol)
    {
        var bindings = new List<ParameterBinding>(32);
        var namedSymbol = (INamedTypeSymbol)symbol;
        var constructorParams = new HashSet<string>();
        var constructors = GetConstructors(namedSymbol).ToArray();

        if (constructors.Length == 1)
        {
            var parameters = constructors[0].Parameters;

            foreach (var parameter in parameters)
            {
                bindings.Add(new ParameterBinding(
                    ParameterTarget.ConstructorParameter,
                    parameter.Type,
                    parameter.GetBindingId(),
                    parameter.Name));

                if (symbol.IsRecord)
                {
                    constructorParams.Add(parameter.Name);
                }
            }
        }

        foreach (var member in symbol.GetMembers())
        {
            switch (member)
            {
                case { Kind: not (SymbolKind.Property or SymbolKind.Field)}:
                    continue;
                
                case { DeclaredAccessibility: not Accessibility.Public }:
                    continue;
                
                case IPropertySymbol { SetMethod: null }:
                    continue;
                
                case IFieldSymbol { IsReadOnly: true }:
                    continue;
            }

            if (constructorParams.Contains(member.Name))
                continue;

            var memberType = member switch
            {
                IPropertySymbol property => property.Type,
                IFieldSymbol field => field.Type,
                _ => throw new InvalidOperationException()
            };
            
            bindings.Add(new ParameterBinding(
                ParameterTarget.Member, 
                memberType, 
                member.GetBindingId(),
                member.Name));
        }
        
        return bindings;
    }

    private static string GetBindingId(this ISymbol symbol)
    {
        var attributes = symbol.GetAttributes();
        var bindToAttribute = attributes.FirstOrDefault(attribute => attribute
            .AttributeClass?.GetGenericTypeName() == "Vertical.Cli.Binding.BindToAttribute");

        return bindToAttribute?.ConstructorArguments[0].Value as string ?? symbol.Name;
    }

    private static IEnumerable<IMethodSymbol> GetConstructors(INamedTypeSymbol namedSymbol)
    {
        foreach (var constructor in namedSymbol.Constructors)
        {
            var parameters = constructor.Parameters;

            if (namedSymbol.IsRecord && parameters.Length == 1 && parameters[0].Type.Equals(namedSymbol,
                    SymbolEqualityComparer.Default))
            {
                // Skip compiler generated copy constructor
                continue;
            }

            if (constructor.DeclaredAccessibility != Accessibility.Public)
                continue;

            yield return constructor;
        }
    }

    private static string GetGenericTypeName(this ISymbol symbol)
    {
        return symbol.ContainingNamespace.Name == string.Empty
            ? symbol.Name
            : $"{symbol.ContainingNamespace.ToDisplayString()}.{symbol.Name}";
    }
}