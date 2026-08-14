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
                    INamedTypeSymbol { TypeKind: TypeKind.Interface } type => type,
                    IParameterSymbol { Type: { TypeKind: TypeKind.Interface } parameterType } => parameterType,
                    _ => null
                })
            .Where(result => result is not null);

        var conversionTargetsProvider = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                NamingConvention.GeneratedConversionAttributeMetadataName,
                predicate: static (_, _) => true,
                transform: static (context, _) => GetTargetConversionTypeSymbol(context))
            .Where(result => result is not null);

        var collected = context
            .CompilationProvider
            .Combine(typesProvider.Collect())
            .Combine(conversionTargetsProvider.Collect());
        
        context.RegisterSourceOutput(collected, (productionContext, source) =>
        {
            var typeModels = source
                .Left
                .Right
                .Distinct(SymbolEqualityComparer.Default)
                .Cast<ITypeSymbol>()
                .Select(typeSymbol => new TypeModel(typeSymbol))
                .ToArray();

            var conversionTargets = source
                .Right
                .Distinct(SymbolEqualityComparer.Default)
                .Cast<ITypeSymbol>()
                .ToArray();

            var code = CodeGenerator.Generate(new GenerationInfo(typeModels, conversionTargets));
            
            productionContext.AddSource("CommandLineApplicationExtensions.g.cs", code);
        });
    }

    private static ITypeSymbol? GetTargetConversionTypeSymbol(GeneratorAttributeSyntaxContext context)
    {
        var symbol = context.TargetSymbol;

        if (symbol is not IParameterSymbol parameterSymbol)
            return null;

        return parameterSymbol.Type switch
        {
            INamedTypeSymbol
            {
                FullMetadataName: NamingConvention.DirectiveEventInfoMetadataName,
                TypeArguments.Length: 1
            } eventInfoSymbol => eventInfoSymbol.TypeArguments[0],
            _ => null
        };
    }
}