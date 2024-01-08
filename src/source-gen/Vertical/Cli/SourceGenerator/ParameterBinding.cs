using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public class ParameterBinding
{
    public ParameterBinding(ParameterTarget target, 
        ITypeSymbol bindingType, 
        string bindingId,
        string metadataName)
    {
        BindingType = bindingType;
        BindingId = bindingId;
        MetadataName = metadataName;
        Target = target;
    }

    public ITypeSymbol BindingType { get; }
    
    public string BindingId { get; }
    public string MetadataName { get; }

    public ParameterTarget Target { get; }

    public string GetContextMethod(string contextVariable)
    {
        if (BindingType.TypeKind == TypeKind.Array)
        {
            var elementType = ((IArrayTypeSymbol)BindingType).ElementType;
            return $"{contextVariable}.GetValues<{elementType.ToFullName()}>(\"{BindingId}\").ToArray()";
        }

        return BindingType.IsSupportedGenericCollectionType()
            ? CreateGenericCollectionContextMethod(contextVariable)
            : $"{contextVariable}.GetValue<{BindingType.ToFullName()}>(\"{BindingId}\")";
    }

    private string CreateGenericCollectionContextMethod(string contextVariable)
    {
        var genericTypeName = ((INamedTypeSymbol)BindingType).TypeArguments[0].ToFullName();
        var baseValueMethod = $"{contextVariable}.GetValues<{genericTypeName}>(\"{BindingId}\")";

        switch (BindingType.Name)
        {
            case "List":
            case "IList":
            case "ICollection":
                return $"{baseValueMethod}.ToList()";
            
            case "IReadOnlyList":
            case "IReadOnlyCollection":
            case "IEnumerable":
                return $"{baseValueMethod}.ToArray()";
            
            case "HashSet":
            case "ISet":
            case "IReadOnlySet":
                return $"new global::System.Collections.Generic.HashSet<{genericTypeName}>({baseValueMethod})";
            
            case "SortedSet":
                return $"new global::System.Collections.Generic.SortedSet<{genericTypeName}>({baseValueMethod})";
            
            case "Stack":
                return $"new global::System.Collections.Generic.Stack<{genericTypeName}>({baseValueMethod})";
            
            case "Queue":
                return $"new global::System.Collections.Generic.Queue<{genericTypeName}>({baseValueMethod})";
            
            default:
                throw new InvalidOperationException();
        }
    }
}