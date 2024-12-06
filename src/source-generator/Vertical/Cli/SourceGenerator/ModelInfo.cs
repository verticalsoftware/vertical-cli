using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public sealed class ModelInfo(
    ITypeSymbol symbol,
    bool include,
    BindingInfo[] constructorBindings, 
    BindingInfo[] propertyBindings)
{
    public static ModelInfo Create(ITypeSymbol symbol)
    {
        var constructorBindings = GetConstructorBindings(symbol);
        
        return new ModelInfo(symbol, 
            GetIncludeFlag(symbol),
            constructorBindings, 
            GetPropertyBindings(symbol, constructorBindings));
    }

    public ITypeSymbol Symbol { get; } = symbol;
    
    public bool Include { get; } = include;
    
    public BindingInfo[] ConstructorBindings { get; } = constructorBindings;

    public BindingInfo[] PropertyBindings { get; } = propertyBindings;

    private static bool GetIncludeFlag(ITypeSymbol model)
    {
        if (model is not INamedTypeSymbol namedSymbol)
            return false;

        return namedSymbol
            .Constructors
            .Count(ctor => ctor.DeclaredAccessibility == Accessibility.Public) == 1;
    }


    private static BindingInfo[] GetConstructorBindings(ITypeSymbol model)
    {
        if (model is not INamedTypeSymbol nameSymbol)
            return [];
        
        var constructor = nameSymbol
            .Constructors
            .FirstOrDefault(ctor => ctor is
            {
                DeclaredAccessibility: Accessibility.Public,
                Parameters.Length: > 0
            });

        return constructor?
            .Parameters
            .Select(parameter => BindingInfo.Create(parameter.Name, parameter.Type))
            .ToArray() ?? [];
    }

    private static BindingInfo[] GetPropertyBindings(ITypeSymbol model, IEnumerable<BindingInfo> constructorBindings)
    {
        var propertyNameSet = new HashSet<string>(constructorBindings.Select(param => param.Name));
        var properties = new List<BindingInfo>(32);

        for (var type = model; type != null; type = type.BaseType)
        {
            if (type is { Name: "Object", ContainingNamespace.Name: "System" })
                break;
            
            properties.AddRange(type
                .GetMembers()
                .Where(member => member is IPropertySymbol
                {
                    DeclaredAccessibility: Accessibility.Public,
                    IsReadOnly: false
                })
                .Cast<IPropertySymbol>()
                .Where(prop => propertyNameSet.Add(prop.Name))
                .Select(prop => BindingInfo.Create(prop.Name, prop.Type)));
        }

        return properties.ToArray();
    }
}