using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using ArgumentSyntax = Vertical.Cli.Parsing.ArgumentSyntax;

namespace Vertical.Cli.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ArityAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        
        context.RegisterSyntaxNodeAction(action: AnalyzeAddArgumentMethod, SyntaxKind.SimpleMemberAccessExpression);
        context.RegisterSyntaxNodeAction(action: AnalyzeOptionArgumentMethod, SyntaxKind.SimpleMemberAccessExpression);
    }

    private static void AnalyzeAddArgumentMethod(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.Node is not MemberAccessExpressionSyntax { Name.Identifier.Text: "Argument" } accessExpressionSyntax)
            return;

        if (!TryGetMethodSymbol(ctx, accessExpressionSyntax, out var methodSymbol))
            return;
        
        // Get the arity value
        var invocation = (InvocationExpressionSyntax)accessExpressionSyntax.Parent!;
        var arity = GetArityValue(invocation, 1, "One", out var location);
        location ??= invocation.GetLocation();
        
        // Get the property type
        var isMultiArityType = IsMultiArityPropertyType(methodSymbol.Parameters);
        
        ReportDiagnostics(ctx, isMultiArityType, arity, location);
    }

    private static void AnalyzeOptionArgumentMethod(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.Node is not MemberAccessExpressionSyntax { Name.Identifier.Text: "AddOption" } accessExpressionSyntax)
            return;
        
        if (!TryGetMethodSymbol(ctx, accessExpressionSyntax, out var methodSymbol))
            return;
        
        // Get the arity value
        var invocation = (InvocationExpressionSyntax)accessExpressionSyntax.Parent!;
        var arity = GetArityValue(invocation, 2, "ZeroOrOne", out var location);
        location ??= invocation.GetLocation();
        
        // Get the property type
        var isMultiArityType = IsMultiArityPropertyType(methodSymbol.Parameters);
        
        ReportDiagnostics(ctx, isMultiArityType, arity, location);
    }

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [Descriptors.VCLI0002, Descriptors.VCLI0003];

    private static bool TryGetMethodSymbol(
        SyntaxNodeAnalysisContext ctx,
        MemberAccessExpressionSyntax accessExpressionSyntax,
        out IMethodSymbol symbol)
    {
        symbol = null!;

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
            } methodSymbol)
        {
            return false;
        }

        symbol = methodSymbol;
        return true;
    }
    
    private static void ReportDiagnostics(SyntaxNodeAnalysisContext ctx, 
        bool isMultiArityType, 
        string arity,
        Location location)
    {
        switch (isMultiArityType, arityValue: arity)
        {
            case { arityValue: "ZeroOrOne", isMultiArityType: false }:
            case { arityValue: "One", isMultiArityType: false }:
            case { arityValue: "ZeroOrMany", isMultiArityType: true }:
            case { arityValue: "OneOrMany", isMultiArityType: true }:
                return;
            
            case { arityValue: "ZeroOrOne" }:
            case { arityValue: "One" }:
                ctx.ReportDiagnostic(Diagnostic.Create(
                    descriptor: Descriptors.VCLI0002,
                    location: location));
                break;
            
            default:
                ctx.ReportDiagnostic(Diagnostic.Create(
                    descriptor: Descriptors.VCLI0003,
                    location: location));
                break;
        }
    }

    private static bool IsMultiArityPropertyType(ImmutableArray<IParameterSymbol> parameters)
    {
        var expressionParameter = parameters.First(p => p.Name == "binding");
        var funcType = ((INamedTypeSymbol)expressionParameter.Type).TypeArguments[0];
        var propertyType = ((INamedTypeSymbol)funcType).TypeArguments[1];
        
        return propertyType switch
        {
            IArrayTypeSymbol => true,
            INamedTypeSymbol
            {
                ContainingNamespace:
                {
                    Name: "Generic" or "Immutable",
                    ContainingNamespace:
                    {
                        Name: "Collections",
                        ContainingNamespace.Name: "System"
                    }
                }
            } => true,
            _ => false
        };
    } 

    private static string GetArityValue(InvocationExpressionSyntax methodSyntax,
        int paramIndex,
        string defaultValue,
        out Location? location)
    {
        location = default!;

        var arguments = methodSyntax.ArgumentList.Arguments;
        var namedArgument = arguments.FirstOrDefault(arg =>
            arg is { NameColon.Expression: IdentifierNameSyntax { Identifier.Text: "arity" } });
        var resolvedArgument = namedArgument ??
                               (paramIndex < arguments.Count ? arguments[paramIndex] : null);

        if (resolvedArgument == null)
            return defaultValue;

        var expression = resolvedArgument.Expression;
        
        if (expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.Text: "Arity" }
            } constantExpression)
        {
            location = constantExpression.GetLocation();
            return constantExpression.Name.Identifier.Text;
        }

        return defaultValue;
    }
}