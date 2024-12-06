using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vertical.Cli.SourceGenerator;

[Generator]
public class CliApplicationExtensionsGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => IsReferenceSyntaxNode(node),
                transform: static (syntaxContext, _) => TransformReferenceSyntaxNode(syntaxContext))
            .Where(node => node is not null);

        var collected = context
            .CompilationProvider
            .Combine(models.Collect());
        
        context.RegisterSourceOutput(collected, (prodContext, source) =>
        {
            if (source.Right.Length == 0)
                return;
            
            const string file = "CliApplicationExtensions.g.cs";
            var generatedCode = SourceBuilder.Build(source.Right);
            
            prodContext.AddSource(file, generatedCode);
        });
    }

    private static ITypeSymbol? TransformReferenceSyntaxNode(GeneratorSyntaxContext syntaxContext)
    {
        var symbolInfo = syntaxContext.SemanticModel.GetSymbolInfo(syntaxContext.Node);

        if (symbolInfo.Symbol is not IMethodSymbol
            {
                Name: "MapModel",
                TypeParameters.Length: 1
            } methodSymbol)
            return null;

        if (methodSymbol.ContainingType is not ITypeSymbol
            {
                Name: "CliApplicationBuilder",
                ContainingNamespace:
                {
                    Name: "Configuration",
                    ContainingNamespace:
                    {
                        Name: "Cli",
                        ContainingNamespace.Name: "Vertical"
                    }
                }
            })
            return null;

        return methodSymbol.TypeArguments[0];
    }

    private static bool IsReferenceSyntaxNode(SyntaxNode node)
    {
        var result = node is MemberAccessExpressionSyntax
        {
            Name.Identifier.Text: "MapModel"
        };

        if (!result) return false;

        return true;
    }
}