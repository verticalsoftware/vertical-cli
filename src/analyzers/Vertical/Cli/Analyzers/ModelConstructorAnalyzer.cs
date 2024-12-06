using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vertical.Cli.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ModelConstructorAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        
        context.RegisterSyntaxNodeAction(action: AnalyzeNodeType, SyntaxKind.SimpleMemberAccessExpression);
    }

    private static void AnalyzeNodeType(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.Node is not MemberAccessExpressionSyntax { Name.Identifier.Text: "MapModel" } methodSyntax)
            return;

        if (ctx.SemanticModel.GetSymbolInfo(ctx.Node).Symbol is not IMethodSymbol
            {
                ContainingNamespace:
                {
                    Name: "Configuration",
                    ContainingNamespace:
                    {
                        Name: "Cli",
                        ContainingNamespace.Name: "Vertical"
                    }
                }
            } methodSymbol)
            return;

        var modelType = (INamedTypeSymbol)methodSymbol.TypeArguments[0];
        var constructorCount = modelType
            .Constructors
            .Count(ctor => ctor.DeclaredAccessibility == Accessibility.Public);

        if (constructorCount == 1)
            return;

        var typeArgumentSyntax = ((GenericNameSyntax)methodSyntax.Name).TypeArgumentList.Arguments[0];
        
        ctx.ReportDiagnostic(Diagnostic.Create(
            descriptor: Descriptors.VCLI0001,
            location: typeArgumentSyntax.GetLocation(),
            messageArgs: []));
    }

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [Descriptors.VCLI0001];
}