using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vertical.Cli.SourceGenerator;

[Generator]
public class RootCommandExtensionsGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => IsCommandReferenceSyntaxNode(node),
                transform: static (syntaxContext, _) => TransformReferenceSyntaxNode(syntaxContext))
            .Where(node => node is not null);

        var collected = context
            .CompilationProvider
            .Combine(models.Collect());
        
        context.RegisterSourceOutput(collected, (prodContext, source) =>
        {
            var definitions = source
                .Right
                .Where(definition => definition is not null)
                .Cast<CommandDefinition>()
                .ToArray();

            var model = new SourceGenerationModel(definitions);
            var builder = new SourceBuilder(model!);
            
            prodContext.AddSource("RootCommandExtensions.g.cs", builder.Generate());
        });
    }

    private static bool IsCommandReferenceSyntaxNode(SyntaxNode node)
    {
        if (node is not ObjectCreationExpressionSyntax objectCreationSyntax)
            return false;

        var type = objectCreationSyntax.Type.GetSimpleTypeName();
        return type is "RootCommand" or "SubCommand";
    }

    private static CommandDefinition? TransformReferenceSyntaxNode(GeneratorSyntaxContext syntaxContext)
    {
        if (syntaxContext.Node is not ObjectCreationExpressionSyntax objectCreationSyntax)
            return null;
     
        var symbol = syntaxContext.SemanticModel.GetSymbolInfo(objectCreationSyntax.Type).Symbol;

        if (symbol is not INamedTypeSymbol
            {
                ContainingAssembly.Name: "Vertical.Cli",
                ContainingNamespace:
                {
                    Name: "Configuration",
                    ContainingNamespace:
                    {
                        Name: "Cli",
                        ContainingNamespace.Name: "Vertical"
                    }
                },
                Name: "RootCommand" or "SubCommand"
            } typeSymbol)
            return null;

        var genericParameterTypes = typeSymbol.TypeArguments;

        if (genericParameterTypes[0] is
            {
                ContainingAssembly.Name: "Vertical.Cli",
                ContainingNamespace:
                {
                    Name: "Configuration",
                    ContainingNamespace:
                    {
                        Name: "Cli",
                        ContainingNamespace.Name: "Vertical"
                    }
                },
                Name: "Empty"
            }) 
            return null;

        var isRootDefinition = typeSymbol.Name == "RootCommand";
        
        return new CommandDefinition(
            (INamedTypeSymbol) genericParameterTypes[0], 
            (INamedTypeSymbol) genericParameterTypes[1],
            isRootDefinition);
    }
}