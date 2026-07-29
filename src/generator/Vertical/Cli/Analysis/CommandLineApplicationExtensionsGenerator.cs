using Microsoft.CodeAnalysis;

namespace Vertical.Cli.Analysis;

[Generator]
public class CommandLineApplicationExtensionsGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typesProvider = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                NamingConvention.GeneratedBindingAttributeMetadataName,
                predicate: static (_, _) => true,
                transform: static (context, _) => context.TargetSymbol switch
                {
                    INamedTypeSymbol { IsRecordOrInterface: true } type => type,
                    IParameterSymbol { Type: { IsRecordOrInterface: true } parameterType } => parameterType,
                    _ => null
                })
            .Where(result => result is not null);

        var collected = context
            .CompilationProvider
            .Combine(typesProvider.Collect());
        
        context.RegisterSourceOutput(collected, (productionContext, source) =>
        {
            var typeModels = source
                .Right
                .Distinct(SymbolEqualityComparer.Default)
                .Cast<ITypeSymbol>()
                .Select(typeSymbol => new TypeModel(typeSymbol))
                .ToArray();

            var code = CodeGenerator.Generate(typeModels);
            
            productionContext.AddSource("CommandLineApplicationExtensions.g.cs", code);
        });
    }
}