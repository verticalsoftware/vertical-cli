using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public class ModelTypeInfo : IEquatable<ModelTypeInfo>
{
    private sealed class ComparerImpl : IEqualityComparer<ModelTypeInfo>
    {
        /// <inheritdoc />
        public bool Equals(ModelTypeInfo x, ModelTypeInfo y) => x.TypeSymbol
            .Equals(y.TypeSymbol, SymbolEqualityComparer.Default);

        /// <inheritdoc />
        public int GetHashCode(ModelTypeInfo obj) => SymbolEqualityComparer.Default.GetHashCode(obj.TypeSymbol);
    }
    
    public ModelTypeInfo(ITypeSymbol typeSymbol)
    {
        var derivesFromModelType = typeSymbol.TypeKind == TypeKind.Interface || typeSymbol.IsAbstract;
        
        TypeSymbol = typeSymbol;
        ImplementationTypeName = $"{typeSymbol.Name}_Impl{NewIdentifier}";
        DeclarableProperties = GetDeclarableProperties(typeSymbol);
        BaseImplementationTypeName = derivesFromModelType
            ? $"{typeSymbol}"
            : null;
        InitializerTypeName = derivesFromModelType
            ? ImplementationTypeName
            : $"{typeSymbol}";
        InitializerParameters = GetConstructorParameters(typeSymbol);
        InitializerProperties = GetInitializerProperties(typeSymbol);
        RequiresActivation = GetRequiresActivation(typeSymbol);
        AssignableProperties = GetAssignableProperties(typeSymbol);
    }

    public bool RequiresActivation { get; set; }

    public static readonly IEqualityComparer<ModelTypeInfo> Comparer = new ComparerImpl();

    public string ImplementationType => TypeSymbol switch
    {
        { IsRecord: true } => "record",
        { TypeKind: TypeKind.Interface } => "interface",
        _ => "class"
    };

    public ImmutableArray<IParameterSymbol> InitializerParameters { get; }

    public IPropertySymbol[] InitializerProperties { get; }

    public IPropertySymbol[] DeclarableProperties { get; }

    public IPropertySymbol[] AssignableProperties { get; }

    public string ImplementationTypeName { get; }

    public string? BaseImplementationTypeName { get; }

    public string InitializerTypeName { get; }

    public ITypeSymbol TypeSymbol { get; }

    /// <inheritdoc />
    public bool Equals(ModelTypeInfo other) => TypeSymbol.Equals(other.TypeSymbol, SymbolEqualityComparer.Default);

    public string NewIdentifier => Guid.NewGuid().ToString()[^8..].ToUpper();

    private static IPropertySymbol[] GetDeclarableProperties(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsRecord)
            return [];

        return typeSymbol
            .GetMembers()
            .Where(member => member is IPropertySymbol propertySymbol && propertySymbol.IsDeclarable())
            .Cast<IPropertySymbol>()
            .ToArray();
    }

    private static IPropertySymbol[] GetInitializerProperties(ITypeSymbol typeSymbol)
    {
        return GetDeclarableProperties(typeSymbol);
    }

    private static IPropertySymbol[] GetAssignableProperties(ITypeSymbol typeSymbol)
    {
        return typeSymbol
            .GetMembers()
            .TakeWhile(_ => !typeSymbol.IsRecord)
            .Where(member => member is IPropertySymbol propertySymbol && propertySymbol.IsAssignable())
            .Cast<IPropertySymbol>()
            .ToArray();
    }

    private static ImmutableArray<IParameterSymbol> GetConstructorParameters(ITypeSymbol typeSymbol)
    {
        if (!typeSymbol.IsRecord)
            return [];

        var constructor = (IMethodSymbol)typeSymbol
            .GetMembers()
            .Single(member => member is IMethodSymbol { MethodKind: MethodKind.Constructor } constructor
                              && !IsRecordCopyConstructor(constructor));

        return constructor.Parameters;
    }

    private static bool IsRecordCopyConstructor(IMethodSymbol methodSymbol)
    {
        return methodSymbol.Parameters.Length == 1 && methodSymbol
            .Parameters[0]
            .Type
            .Equals(methodSymbol.ContainingType, SymbolEqualityComparer.Default);
    }

    private static bool GetRequiresActivation(ITypeSymbol typeSymbol)
    {
        return typeSymbol is
        {
            TypeKind: TypeKind.Class,
            IsRecord: false,
            IsAbstract: false
        } && typeSymbol.GetMembers().Any(member => member is IMethodSymbol
        {
            MethodKind: MethodKind.Constructor,
            Parameters.Length: > 0
        });
    }
}