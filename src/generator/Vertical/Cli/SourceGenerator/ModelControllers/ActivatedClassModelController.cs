using Microsoft.CodeAnalysis;
using Vertical.Cli.SourceGenerator.Utilities;

namespace Vertical.Cli.SourceGenerator.ModelControllers;

public sealed class ActivatedClassModelController(
    ITypeSymbol typeSymbol,
    BindingExpressionHelper expressionHelper)
    : ModelController(typeSymbol, expressionHelper)
{
    /// <inheritdoc />
    protected override string BaseTypeSyntax => string.Empty;

    /// <inheritdoc />
    protected override void WriteBindMethodBody(CodeFormatter formatter)
    {
        formatter
            .WriteLine($"var model = {ExpressionHelper.ContextParameter}.ActivateInstance();")
            .WriteLine();

        var properties = Properties
            .Where(property => property is
            {
                DeclaredAccessibility: not Accessibility.Private,
                SetMethod: not null
            });

        foreach (var property in properties)
        {
            formatter.WriteLine($"model.{property.Name} = {ExpressionHelper.GetBindingExpression(property)};");
        }

        formatter
            .WriteLine()
            .WriteLine("return model;");
    }
}