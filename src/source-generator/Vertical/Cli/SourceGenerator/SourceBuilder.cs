using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

internal static class SourceBuilder
{
    internal static string Build(ImmutableArray<ITypeSymbol?> modelSymbols)
    {
        var typeSymbolSet = new HashSet<ITypeSymbol>(
            modelSymbols
                .Where(symbol => symbol is not null)
                .Cast<ITypeSymbol>(),
            SymbolEqualityComparer.Default);

        if (typeSymbolSet.Count == 0)
        {
            return "// No models configured";
        }

        var src = new StringBuilder(50000);
        
        WriteHeader(src);
        WriteNamespace(src);
        WriteExtensionClass(src, modelSymbols);

        return src.ToString();
    }

    private static void WriteNamespace(StringBuilder src)
    {
        src.AppendLine("namespace Vertical.Cli.Configuration;");
        src.AppendLine();
        
    }

    private static void WriteExtensionClass(StringBuilder src, IEnumerable<ITypeSymbol> modelTypes)
    {
        src.AppendLine("public static class CliApplicationExtensions");
        src.AppendLine("{");
        WriteExtensionMethod(src, modelTypes);
        src.AppendLine("}");
    }

    private static void WriteExtensionMethod(StringBuilder src, IEnumerable<ITypeSymbol> modelTypes)
    {
        src.AppendLine("    /// <summary>");
        src.AppendLine("    /// Parses the arguments, builds a model if needed, and then invokes the handler defined by the application.");
        src.AppendLine("    /// </summary>");
        src.AppendLine("    /// <param name=\"app\">The instance that contains the application's configuration.</param>");
        src.AppendLine("    /// <param name=\"args\">Arguments to parse. Typically these are received from the command line.</param>");
        src.AppendLine("    /// <param name=\"cancellationToken\">Token that can be observed for cancellation by application handlers.</param>");
        src.AppendLine("    /// <returns>A task representing the asynchronous operation, which wraps the result returned by the handler.</returns>");
        src.AppendLine("    /// <exception cref=\"Vertical.Cli.CliArgumentException\">The provided argument input was invalid.</exception>");
        src.AppendLine("    /// <exception cref=\"Vertical.Cli.CliConfigurationException\">The application is not configured correctly.</exception>");
        src.AppendLine("    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vertical.Cli.SourceGenerator\", \"1.0.0\")]");
        src.AppendLine("    public static async global::System.Threading.Tasks.Task<int> InvokeAsync(");
        src.AppendLine("        this global::Vertical.Cli.Configuration.CliApplication app,");
        src.AppendLine("        global::System.String[] args,");
        src.AppendLine("        global::System.Threading.CancellationToken cancellationToken = default)");
        src.AppendLine("    {");
        src.AppendLine("        var context = global::Vertical.Cli.CliEngine.GetBindingContext(app, args);");
        src.AppendLine();
        src.AppendLine("        // Routes to CallSite<EmptyModel>, CallSite<BindingContext>, etc.");
        src.AppendLine("        if (await context.TryInvokeInternalCallSite(cancellationToken) is { } result)");
        src.AppendLine("            return result;");
        src.AppendLine();

        WriteCallSites(src, modelTypes);
        
        src.AppendLine("        throw new global::System.InvalidOperationException(\"Failed to invoke call site\");");
        src.AppendLine("    }");
    }

    private static void WriteCallSites(StringBuilder src, IEnumerable<ITypeSymbol> modelTypes)
    {
        var id = 1;
        
        foreach (var type in modelTypes)
        {
            WriteCallSite(src, type, id++);
            src.AppendLine();
        }
    }

    private static void WriteCallSite(StringBuilder src, ITypeSymbol type, int modelId)
    {
        var modelInfo = ModelInfo.Create(type);

        if (!modelInfo.Include)
        {
            src.AppendLine($"        // {type.Name} excluded (has no public constructor)");
            return;
        }

        var callSiteName = $"callSite{modelId}";
        var modelName = $"model{modelId}";

        src.AppendLine($"        // Call to {type.Name} handlers");
        src.AppendLine($"        if (context.TryGetCallSite<{Format(type)}>(out var {callSiteName}))");
        src.AppendLine("        {");

        WriteConverters(src, modelInfo);
        WriteModelBinding(src, modelInfo, modelName);

        src.AppendLine($"            return await {callSiteName}({modelName}, cancellationToken);");
        src.AppendLine("        }");
    }

    private static void WriteModelBinding(StringBuilder src, ModelInfo modelInfo, string modelName)
    {
        src.Append($"            var {modelName} = new {modelInfo.Symbol.ToFqn()}");

        if (modelInfo is { ConstructorBindings.Length: 0, PropertyBindings.Length: 0 })
        {
            src.AppendLine("();");
            return;
        }

        if (modelInfo.ConstructorBindings.Length > 0)
        {
            src.AppendLine("(");
            WriteBindingCsv(src, modelInfo.ConstructorBindings, param => $"{param.Name}: context.{param.BindingExpression}");
            src.Append(")");
        }

        if (modelInfo.PropertyBindings.Length > 0)
        {
            src.AppendLine();
            src.Append("            {");
            src.AppendLine();
            WriteBindingCsv(src, modelInfo.PropertyBindings, prop => $"{prop.Name} = context.{prop.BindingExpression}");
            src.AppendLine();
            src.Append("            }");
        }

        src.AppendLine(";");
        src.AppendLine();
    }

    private static void WriteConverters(StringBuilder src, ModelInfo modelInfo)
    {
        // Need converters for all constructor and public properties
        var expressions = modelInfo
            .ConstructorBindings
            .Select(ctor => ctor.ConverterExpression)
            .Concat(modelInfo.PropertyBindings.Select(property => property.ConverterExpression))
            .Where(expression => expression != null)
            .Cast<string>()
            .Distinct();

        var len = src.Length;
        
        foreach (var expression in expressions)
        {
            src.AppendLine($"            context.TryAddConverter({expression});");
        }

        if (len != src.Length)
            src.AppendLine();
    }

    private static void WriteHeader(StringBuilder src)
    {
        src.AppendLine("// <auto-generated/>");
        src.AppendLine("#nullable enable");
        src.AppendLine();
    }

    private static string Format(ITypeSymbol type)
    {
        return type.ToDisplayString(format: SymbolDisplayFormat.FullyQualifiedFormat);
    }

    private static void WriteBindingCsv<T>(StringBuilder sb, T[] items, Func<T, string> provider)
    {
        if (items.Length == 0)
            return;
        
        for (var c = 0; c < items.Length; c++)
        {
            if (c > 0)
            {
                sb.AppendLine(",");
            }

            sb.Append("                 ");
            sb.Append(provider(items[c]));
        }
    }
}