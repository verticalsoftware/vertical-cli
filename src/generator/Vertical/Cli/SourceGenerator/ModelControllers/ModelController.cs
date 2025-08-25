using Microsoft.CodeAnalysis;
using Vertical.Cli.SourceGenerator.Utilities;

namespace Vertical.Cli.SourceGenerator.ModelControllers;

public abstract class ModelController(ITypeSymbol typeSymbol, BindingExpressionHelper expressionHelper) 
    : IModelController
{
    public static IModelController? Create(ITypeSymbol typeSymbol, BindingExpressionHelper expressionHelper)
    {
        return typeSymbol switch
        {
            { TypeKind: TypeKind.Interface } => new InterfaceModelController(typeSymbol, expressionHelper),
            { IsRecord: true } => new RecordModelController(typeSymbol, expressionHelper),
            not null when typeSymbol.IsNewTypeConstraintClass() => new ConstructableClassModelController(typeSymbol, expressionHelper),
            not null when typeSymbol.RequiresActivation() => new ActivatedClassModelController(typeSymbol, expressionHelper),
            _ => null
        };
    }
    
    public ITypeSymbol TypeSymbol => typeSymbol;

    public BindingExpressionHelper ExpressionHelper => expressionHelper;

    public string ImplementationTypeName { get; } = $"{typeSymbol.Name}_{TinyId.Next}_Impl";

    protected virtual string BaseTypeSyntax => $" : {TypeSymbol}";

    public void WriteImplementation(CodeFormatter formatter)
    {
        formatter
            .WriteLine("/// <summary>")
            .WriteLine($"/// Binding support for the {typeSymbol} model type")
            .WriteLine("/// </summary>")
            .WriteLine($"private sealed class {ImplementationTypeName}{BaseTypeSyntax}")
            .WriteLine('{')
            .Indent()
            .Track();
        
        WriteDeclaredMembers(formatter);

        if (formatter.HasChanges)
        {
            formatter.WriteLine();
        }
        
        WriteBindMethod(formatter);
        
        formatter
            .UnIndent()
            .WriteLine('}');
    }

    private void WriteBindMethod(CodeFormatter builder)
    {
        builder
            .WriteGeneratedCodeAttribute()
            .WriteLine(
                $"internal static {typeSymbol} Bind(Vertical.Cli.Binding.BindingContext<{typeSymbol}> {expressionHelper.ContextParameter})")
            .WriteLine('{')
            .Indent();
        
        WriteBindMethodBody(builder);

        builder
            .UnIndent()
            .WriteLine('}');
    }

    protected IPropertySymbol[] Properties { get; } = typeSymbol
        .EnumerateSelfAndBaseTypes()
        .SelectMany(type => type.GetMembers().OfType<IPropertySymbol>())
        .ToArray();

    protected abstract void WriteBindMethodBody(CodeFormatter formatter);

    protected virtual void WriteDeclaredMembers(CodeFormatter builder)
    {
    }
}