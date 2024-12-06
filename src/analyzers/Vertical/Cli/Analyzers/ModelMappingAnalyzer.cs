using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vertical.Cli.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ModelMappingAnalyzer : DiagnosticAnalyzer
{
    private sealed class ModelBinding(INamedTypeSymbol type, Location location, string binding)
    {
        public INamedTypeSymbol Type { get; } = type;
        public Location Location { get; } = location;
        public string Binding { get; } = binding;

        /// <inheritdoc />
        public override string ToString() => $"{Type.Name} x => x.{Binding}";
    }

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        
        context.RegisterCompilationStartAction(action: StartAnalysis);
    }

    private void StartAnalysis(CompilationStartAnalysisContext startContext)
    {
        var bindings = new ConcurrentBag<ModelBinding>();
        
        startContext.RegisterSyntaxNodeAction(context => AnalyzeMapModelMethod(context, bindings), 
            SyntaxKind.GenericName);
        
        startContext.RegisterCompilationEndAction(context => AnalyzeBindings(context, bindings));
    }

    private static void AnalyzeBindings(CompilationAnalysisContext context, ConcurrentBag<ModelBinding> bindings)
    {
        var models = bindings.ToLookup(b => b.Type, SymbolEqualityComparer.Default);

        foreach (var group in models)
        {
            var type = (INamedTypeSymbol)group.Key!;
            var bindingNames = new HashSet<string>(group.Select(item => item.Binding));
            var propertyDictionary = type
                .GetMembers()
                .Where(member => member is IPropertySymbol
                {
                    DeclaredAccessibility: Accessibility.Public,
                    IsReadOnly: false
                })
                .Cast<IPropertySymbol>()
                .ToDictionary(property => property.Name);

            var locations = group.Select(g => g.Location).ToArray();

            ReportMissingBindings(context, type, bindingNames, propertyDictionary, locations);
            ReportDerivedBindings(context, type, bindingNames, propertyDictionary, locations);
        }
    }

    private static void ReportMissingBindings(CompilationAnalysisContext context, 
        INamedTypeSymbol type,
        HashSet<string> bindingNames,
        Dictionary<string, IPropertySymbol> propertyDictionary, 
        Location[] locations)
    {
        var missingBindings = propertyDictionary
            .Keys
            .Where(key => !bindingNames.Contains(key));

        var list = string.Join(",", missingBindings);
        if (list.Length == 0)
            return;
        
        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.VCLI0010,
            locations.First(),
            locations.Skip(1),
            properties: null,
            type, list));
    }

    private static void ReportDerivedBindings(CompilationAnalysisContext context,
        INamedTypeSymbol type,
        HashSet<string> bindingNames,
        Dictionary<string, IPropertySymbol> propertyDictionary, 
        Location[] locations)
    {
        var derivedBindings = bindingNames.Where(property => !propertyDictionary.ContainsKey(property));
        var list = string.Join(",", derivedBindings);

        if (list.Length == 0)
            return;
        
        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.VCLI0011,
            locations.First(),
            locations.Skip(1),
            properties: null,
            type, list));
    }

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
    [
        Descriptors.VCLI0010,
        Descriptors.VCLI0011
    ];
    
    private static void AnalyzeMapModelMethod(SyntaxNodeAnalysisContext ctx, ConcurrentBag<ModelBinding> bindings)
    {
        if (ctx.Node is not GenericNameSyntax { Identifier.Text: "MapModel" } methodNameSyntax)
            return;

        if (methodNameSyntax.Parent == null)
            return;

        var symbolInfo = ctx.SemanticModel.GetSymbolInfo(methodNameSyntax.Parent!);
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
            } mapMethodSymbol)
        {
            return;
        }

        if (mapMethodSymbol.TypeArguments[0] is not INamedTypeSymbol modelType)
            return;
        
        if (methodNameSyntax.Parent.Parent is not InvocationExpressionSyntax mapMethodInvocation)
            return;

        if (mapMethodInvocation.ArgumentList.Arguments[0].Expression is not LambdaExpressionSyntax lambdaExpression)
            return;

        foreach (var node in lambdaExpression.DescendantNodes())
        {
            switch (node)
            {
                case InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax
                    {
                        Name.Identifier.Text: "Argument" or "Option" or "Switch" or "ValueBinding"
                    }
                } addMethodSyntax:
                    
                    var bindingArgument = (LambdaExpressionSyntax) addMethodSyntax.ArgumentList.Arguments[0].Expression;
                    var propertyAccessor = (MemberAccessExpressionSyntax)bindingArgument.ExpressionBody!;
                    var binding = propertyAccessor.Name.Identifier.Text;
                    var location = methodNameSyntax.GetLocation();
                    
                    bindings.Add(new ModelBinding(modelType, location, binding));
                    
                    break;
            }
        }
    }
}