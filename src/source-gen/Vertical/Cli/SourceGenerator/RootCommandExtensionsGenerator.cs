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
                predicate: static (node, _) => IsConfigurationSyntaxNode(node),
                transform: static (syntaxContext, _) => TransformConfigurationSyntaxNode(syntaxContext))
            .Where(node => node is not null);

        var collected = context
            .CompilationProvider
            .Combine(models.Collect());
        
        context.RegisterSourceOutput(collected, (prodContext, source) =>
        {
            var commandModels = source
                .Right
                .Where(model => model is not null)
                .Cast<CommandModel>()
                .ToArray();

            if (commandModels.Count(model => model.IsRootCommand) == 0)
                return;

            var generationModel = new GenerationModel(commandModels);
            var sourceContent = SourceBuilder.Build(source.Left, generationModel);
            
            prodContext.AddSource("RootCommandExtensions.g.cs", sourceContent);
        });
    }

    private static bool IsConfigurationSyntaxNode(SyntaxNode node)
    {
        return IsRootCommandCreateExpression(node) || IsConfigureSubCommandExpression(node);
    }

    private static bool IsRootCommandCreateExpression(SyntaxNode node)
    {
        return node is MemberAccessExpressionSyntax
        {
            Expression: IdentifierNameSyntax { Identifier.Text: "RootCommand" } or 
            QualifiedNameSyntax { Right.Identifier.Text: "RootCommand" },
            Name.Identifier.Text: "Create"
        };
    }

    private static bool IsConfigureSubCommandExpression(SyntaxNode node)
    {
        return node is MemberAccessExpressionSyntax
        {
            Name.Identifier.Text: "ConfigureSubCommand"
        };
    }

    private static CommandModel? TransformConfigurationSyntaxNode(GeneratorSyntaxContext syntaxContext)
    {
        var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)syntaxContext.Node;
        var semanticModel = syntaxContext.SemanticModel;

        return TryCreateCommandMetadata(memberAccessExpressionSyntax, semanticModel);
    }
    
    private static CommandModel? TryCreateCommandMetadata(
        SyntaxNode memberAccessExpressionSyntax,
        SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetSymbolInfo(memberAccessExpressionSyntax).Symbol;

        if (symbol is not IMethodSymbol methodSymbol)
            return null;

        var isRootCommand = methodSymbol.Name == "Create";

        return methodSymbol.TryGetCommandBuilderGenericArguments(1, out var modelType, out var resultType)
            ? new CommandModel(isRootCommand: isRootCommand, modelType!, resultType!)
            : null;
    }
}