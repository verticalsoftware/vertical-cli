using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vertical.Cli.SourceGenerator;

[Generator]
public sealed class CommandLineBuilderIncrementalGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var modelTypes = context
            .SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => SelectAttributeNotes(node),
                transform: static (syntaxContext, _) => SelectModelType(syntaxContext))
            .Where(item => item is not null);

        var collected = context
            .CompilationProvider
            .Combine(modelTypes.Collect());
        
        context.RegisterSourceOutput(collected, (productionContext, source) =>
        {
            var generationList = new HashSet<ModelTypeInfo>(source
                    .Right
                    .Where(typeSymbol => typeSymbol is not null)
                    .Select(typeSymbol => new ModelTypeInfo(typeSymbol!)),
                ModelTypeInfo.Comparer);

            if (generationList.Count == 0)
                return;

            var code = new CodeGenerator(generationList).Build();
            
            productionContext.AddSource("CommandLineBuilderExtensions.g.cs", code);
        });
    }

    private static bool SelectAttributeNotes(SyntaxNode node)
    {
        return node is AttributeSyntax attributeSyntax && attributeSyntax.Name.GetSimpleName()
            is "GeneratedBinding" or "GeneratedBindingAttribute";
    }

    private static ITypeSymbol? SelectModelType(GeneratorSyntaxContext syntaxContext)
    {
        var attributeNode = syntaxContext.Node;
        var attributeListNode = attributeNode.Parent;
        var annotatedSyntaxNode = attributeListNode?.Parent;

        if (annotatedSyntaxNode == null)
            return null;

        var symbol = syntaxContext
            .SemanticModel
            .GetDeclaredSymbol(annotatedSyntaxNode);

        var modelType = symbol switch
        {
            ITypeSymbol typeSymbol => typeSymbol,
            IParameterSymbol parameterSymbol => parameterSymbol.Type,
            _ => null
        };
        
        return modelType switch
        {
            { TypeKind: TypeKind.Interface } => modelType,
            { IsRecord: true } => modelType,
            { TypeKind: TypeKind.Class, IsAbstract: false } => modelType,
            _ => null
        };
    }
}