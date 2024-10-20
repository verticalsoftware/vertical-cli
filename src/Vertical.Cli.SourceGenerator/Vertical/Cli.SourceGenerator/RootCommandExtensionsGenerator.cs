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
                predicate: static (node, _) => IsReferenceSyntaxNode(node),
                transform: static (syntaxContext, _) => TransformReferenceSyntaxNode(syntaxContext))
            .Where(node => node is not null);

        var collected = context
            .CompilationProvider
            .Combine(models.Collect());
        
        context.RegisterSourceOutput(collected, (prodContext, source) =>
        {
            const string file = "RootCommandExtensions.g.cs";
            
            var definitions = source
                .Right
                .Where(definition => definition is not null)
                .Cast<CommandDefinition>()
                .ToArray();

            if (definitions.Length == 0)
            {
                prodContext.AddSource(
                    file,
                    "// No commands have been configured in the source application");
                return;
            }

            var model = new SourceGenerationModel(definitions);
            var builder = new SourceBuilder(model!);
            
            prodContext.AddSource(file, builder.Generate());
        });
    }

    private static bool IsReferenceSyntaxNode(SyntaxNode node)
    {
        return IsCommandReferenceSyntaxNode(node) || IsAddSubCommandReferenceSyntaxNode(node);
    }

    private static bool IsCommandReferenceSyntaxNode(SyntaxNode node)
    {
        if (node is not ObjectCreationExpressionSyntax objectCreationSyntax)
            return false;

        var type = objectCreationSyntax.Type.GetSimpleTypeName();
        return type is "RootCommand";
    }

    private static bool IsAddSubCommandReferenceSyntaxNode(SyntaxNode node)
    {
        return node is MemberAccessExpressionSyntax
        {
            Name: GenericNameSyntax
            {
                Identifier.Text: "AddSubCommand",
                TypeArgumentList.Arguments.Count: 1
            }
        };
    }

    private static CommandDefinition? TransformReferenceSyntaxNode(GeneratorSyntaxContext syntaxContext)
    {
        return syntaxContext.Node is ObjectCreationExpressionSyntax
            ? TransformRootCommandReferenceSyntaxNode(syntaxContext)
            : TransformSubCommandReferenceSyntaxNode(syntaxContext);
    }

    private static CommandDefinition? TransformSubCommandReferenceSyntaxNode(GeneratorSyntaxContext syntaxContext)
    {
        if (syntaxContext.Node is not MemberAccessExpressionSyntax memberAccessSyntax)
            return null;

        var symbolInfo = syntaxContext.SemanticModel.GetSymbolInfo(memberAccessSyntax.Name);

        if (symbolInfo.Symbol is not IMethodSymbol
            {
                Name: "AddSubCommand",
                ContainingType:
                {
                    Name: "CliCommand",
                    ContainingNamespace:
                    {
                        Name: "Configuration",
                        ContainingNamespace:
                        {
                            Name: "Cli",
                            ContainingNamespace.Name: "Vertical"
                        }
                    }
                }
            } methodSymbol)
            return null;

        var modelType = methodSymbol.TypeArguments[0];
        return new CommandDefinition((INamedTypeSymbol)modelType, false);
    }

    private static CommandDefinition? TransformRootCommandReferenceSyntaxNode(GeneratorSyntaxContext syntaxContext)
    {
        if (syntaxContext.Node is not ObjectCreationExpressionSyntax objectCreationSyntax)
            return null;
     
        var symbol = syntaxContext.SemanticModel.GetSymbolInfo(objectCreationSyntax.Type).Symbol;

        if (symbol is not INamedTypeSymbol
            {
                ContainingAssembly.Name: "Vertical.Cli",
                ContainingNamespace:
                {
                    Name: "Cli",
                    ContainingNamespace.Name: "Vertical"
                },
                Name: "RootCommand"
            } typeSymbol)
            return null;

        var modelType = typeSymbol.TypeArguments[0];

        if (modelType.HasNoGeneratorBindingAttribute())
            return null;

        var isRootDefinition = typeSymbol.Name == "RootCommand";
        
        return new CommandDefinition(
            (INamedTypeSymbol) modelType, 
            isRootDefinition);
    }
}