using Microsoft.CodeAnalysis;
using Vertical.Cli.SourceGenerator.Utilities;

namespace Vertical.Cli.SourceGenerator.ModelControllers;

public sealed class ConstructableClassModelController(
    ITypeSymbol typeSymbol,
    BindingExpressionHelper expressionHelper)
    : ModelController(typeSymbol, expressionHelper)
{
    /// <inheritdoc />
    protected override void WriteBindMethodBody(CodeFormatter formatter)
    {
        formatter.Write($"return new {TypeSymbol}");

        var properties = Properties
            .Where(property => property is
            {
                DeclaredAccessibility: not Accessibility.Private,
                SetMethod: not null
            })
            .ToArray();

        if (properties.Length == 0)
        {
            formatter.WriteLine("();");
            return;
        }
        
        var list = formatter
            .WriteLine()
            .WriteLine('{')
            .Indent()
            .CreateListWriter();

        var propertyQuery = Properties
            .Where(property => property is
            {
                DeclaredAccessibility: not Accessibility.Private,
                SetMethod: not null
            });

        foreach (var property in propertyQuery)
        {
            list.WriteNext($"{property.Name} = {ExpressionHelper.GetBindingExpression(property)}");
        }

        list
            .Complete()
            .UnIndent()
            .WriteLine("};");
    }
}