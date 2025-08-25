using Microsoft.CodeAnalysis;
using Vertical.Cli.SourceGenerator.Utilities;

namespace Vertical.Cli.SourceGenerator.ModelControllers;

public sealed class InterfaceModelController(
    ITypeSymbol typeSymbol,
    BindingExpressionHelper expressionHelper) 
    : ModelController(typeSymbol, expressionHelper)
{
    /// <inheritdoc />
    protected override void WriteDeclaredMembers(CodeFormatter builder)
    {
        foreach (var property in Properties)
        {
            builder.WritePropertyDeclaration(property);
        }
    }

    /// <inheritdoc />
    protected override void WriteBindMethodBody(CodeFormatter formatter)
    {
        formatter.Write($"return new {ImplementationTypeName}");

        if (Properties.Length == 0)
        {
            formatter.WriteLine("();");
            return;
        }
        
        var list = formatter
            .WriteLine()
            .WriteLine('{')
            .Indent()
            .CreateListWriter();

        foreach (var property in Properties)
        {
            list.WriteNext(b => b.Write($"{property.Name} = {ExpressionHelper.GetBindingExpression(property)}"));
        }

        list
            .Complete()
            .UnIndent()
            .WriteLine("};");
    }
}