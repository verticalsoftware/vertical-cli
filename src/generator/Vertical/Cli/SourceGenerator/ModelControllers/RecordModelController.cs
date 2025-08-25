using Microsoft.CodeAnalysis;
using Vertical.Cli.SourceGenerator.Utilities;

namespace Vertical.Cli.SourceGenerator.ModelControllers;

public sealed class RecordModelController(
    ITypeSymbol typeSymbol,
    BindingExpressionHelper expressionHelper)
    : ModelController(typeSymbol, expressionHelper)
{
    /// <inheritdoc />
    protected override string BaseTypeSyntax => string.Empty;

    /// <inheritdoc />
    protected override void WriteBindMethodBody(CodeFormatter formatter)
    {
        var parameters = TypeSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Single(method => method is
            {
                MethodKind: MethodKind.Constructor,
                DeclaredAccessibility: Accessibility.Public
            })
            .Parameters;

        formatter.Write($"return new {TypeSymbol}");

        if (parameters.Length == 0)
        {
            formatter.WriteLine("();");
            return;
        }

        var list = formatter
            .WriteLine()
            .WriteLine('(')
            .Indent()
            .CreateListWriter();

        foreach (var parameter in parameters)
        {
            list.WriteNext($"{parameter.Name}: {ExpressionHelper.GetBindingExpression(parameter)}");
        }

        list
            .Complete()
            .UnIndent()
            .WriteLine(");");
    }
}