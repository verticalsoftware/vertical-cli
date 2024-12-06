using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class IdentifierAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        
        context.RegisterSyntaxNodeAction(action: AnalyzeIdentifierNodes, SyntaxKind.SimpleMemberAccessExpression);
    }

    private void AnalyzeIdentifierNodes(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.Node is not MemberAccessExpressionSyntax
            {
                Name.Identifier.Text: "AddOption" or "AddSwitch"
            } accessExpressionSyntax)
            return;

        if (ctx.SemanticModel.GetSymbolInfo(accessExpressionSyntax).Symbol is not IMethodSymbol
            {
                ContainingType:
                {
                    Name: "ModelConfiguration",
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
            }) return;

        // Get the identifier names
        if (accessExpressionSyntax.Parent is not InvocationExpressionSyntax invocationSyntax)
            return;

        var arguments = invocationSyntax.ArgumentList.Arguments;
        var identifiersExpression = arguments.FirstOrDefault(arg => arg.NameColon is 
                                        { Name.Identifier.Text: "identifiers" }) ??
                                    arguments[1];

        if (identifiersExpression.Expression is not CollectionExpressionSyntax arraySyntax)
            return;

        var identifierSyntaxes = arraySyntax
            .Elements
            .Where(element => element is ExpressionElementSyntax { Expression: LiteralExpressionSyntax })
            .Cast<ExpressionElementSyntax>()
            .Select(element => (LiteralExpressionSyntax)element.Expression)
            .ToArray();

        if (identifierSyntaxes.Length == 0)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                descriptor: Descriptors.VCLI0005,
                location: arraySyntax.GetLocation()));
        }

        foreach (var identifierSyntax in identifierSyntaxes)
        {
            var argumentSyntax = Parsing.ArgumentSyntax.Parse(identifierSyntax.Token.ValueText);
            if (argumentSyntax.PrefixType != OptionPrefixType.None)
                continue;
            
            ctx.ReportDiagnostic(Diagnostic.Create(
                descriptor: Descriptors.VCLI0004,
                location: identifierSyntax.GetLocation()));
        }
    }

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = 
    [
        Descriptors.VCLI0004,
        Descriptors.VCLI0005
    ];
}