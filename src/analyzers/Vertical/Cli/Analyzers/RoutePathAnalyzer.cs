using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vertical.Cli.Routing;

namespace Vertical.Cli.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RoutePathAnalyzer : DiagnosticAnalyzer
{
    private sealed class RouteSite(string path, Location location)
    {
        public string Path { get; } = path;
        public Location Location { get; } = location;
    }

    private sealed class AppDeclarationSite(string name, Location location)
    {
        public string Name { get; } = name;
        public Location Location { get; } = location;
    }
    
    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        
        context.RegisterCompilationStartAction(action: PerformAnalysis);
    }

    private static void PerformAnalysis(CompilationStartAnalysisContext context)
    {
        var appNameSites = new ConcurrentBag<AppDeclarationSite>();
        var routeSites = new ConcurrentBag<RouteSite>();

        context.RegisterSyntaxNodeAction(nodeContext => TryAddAppDeclarationSite(nodeContext, appNameSites),
            SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(nodeContext => TryAddRouteSite(nodeContext, routeSites),
            SyntaxKind.SimpleMemberAccessExpression);
        context.RegisterCompilationEndAction(compileContext => Analyze(compileContext, appNameSites, routeSites));
    }

    private static void Analyze(CompilationAnalysisContext compileContext, 
        ConcurrentBag<AppDeclarationSite> appNameSites,
        ConcurrentBag<RouteSite> routeSites)
    {
        if (appNameSites.Count != 1)
            return;

        var appName = appNameSites.First().Name;

        foreach (var (path, location) in routeSites.Select(site => (site.Path, site.Location)))
        {
            if (!path.StartsWith(appName))
            {
                compileContext.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.VCLI0009,
                    location,
                    appName));
                continue;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                compileContext.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.VCLI0008,
                    location));
                continue;
            }

            if (RoutePath.TryParse(path, out _))
                continue;
            
            compileContext.ReportDiagnostic(Diagnostic.Create(
                Descriptors.VCLI0007,
                location));
        }
    }

    private static void TryAddAppDeclarationSite(SyntaxNodeAnalysisContext nodeContext, ConcurrentBag<AppDeclarationSite> sites)
    {
        if (nodeContext.Node is not ObjectCreationExpressionSyntax
            {
                Type: IdentifierNameSyntax { Identifier.Text: "CliApplicationBuilder" }
            } creationSyntax)
            return;

        var symbolInfo = nodeContext.SemanticModel.GetSymbolInfo(creationSyntax);
        if (symbolInfo.Symbol is not IMethodSymbol
            {
                ContainingType:
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
                }
            })
            return;

        var appNameArgument = creationSyntax.ArgumentList!.Arguments[0];
        if (appNameArgument.Expression is not LiteralExpressionSyntax literal)
            return;
        
        sites.Add(new AppDeclarationSite(literal.Token.ValueText, appNameArgument.GetLocation()));
    }

    private static void TryAddRouteSite(SyntaxNodeAnalysisContext nodeContext, ConcurrentBag<RouteSite> sites)
    {
        if (nodeContext.Node is not MemberAccessExpressionSyntax
            {
                Name.Identifier.Text: "Route" or "RouteAsync"
            } accessExpressionSyntax)
            return;
        
        if (nodeContext.SemanticModel.GetSymbolInfo(accessExpressionSyntax).Symbol is not IMethodSymbol
            {
                ContainingType:
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
                }
            }) return;
        
        // Get the route names
        if (accessExpressionSyntax.Parent is not InvocationExpressionSyntax invocationSyntax)
            return;
        
        var arguments = invocationSyntax.ArgumentList.Arguments;
        var pathArgument = arguments.FirstOrDefault(arg => arg.NameColon?.ColonToken.Text == "path")
                           ?? arguments[0];
        
        if (pathArgument.Expression is MemberAccessExpressionSyntax
            {
                Expression: PredefinedTypeSyntax { Keyword.Text: "string" },
                Name.Identifier.Text: "Empty"
            })
        {
            
            sites.Add(new RouteSite(string.Empty, pathArgument.GetLocation()));
            return;
        }
        
        if (pathArgument.Expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.Text: "String" },
                Name.Identifier.Text: "Empty"
            })
        {
            sites.Add(new RouteSite(string.Empty, pathArgument.GetLocation()));
            return;
        }

        if (pathArgument.Expression is not LiteralExpressionSyntax literalSyntax)
            return;

        var path = literalSyntax.Token.ValueText;
        sites.Add(new RouteSite(path, pathArgument.GetLocation()));
    }

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        Descriptors.VCLI0007,
        Descriptors.VCLI0008,
        Descriptors.VCLI0009
    ];
}