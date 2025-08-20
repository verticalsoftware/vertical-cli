using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public sealed class CodeGenerator
{
    public CodeGenerator(HashSet<ModelTypeInfo> modelEntries)
    {
        _modelEntries = modelEntries;
    }

    private readonly HashSet<ModelTypeInfo> _modelEntries;
    private readonly CodeFormattedStringBuilder _builder = new();
    private readonly BindingExpressionHelper _expressionHelper = new("context");

    public string Build()
    {
        WriteHeader();
        WriteExtensionClass();
        
        return _builder.ToString();
    }

    private void WriteExtensionClass()
    {
        _builder
            .WriteLine("/// <summary>")
            .WriteLine("/// Source generated extension class for the <see cref=\"Vertical.Cli.CommandLineBuilder\"/> class")
            .WriteLine("/// </summary>")
            .WriteLine("public static class CommandLineBuilderExtensions")
            .WriteCodeBlockStart();

        WriteModelImplementations();
        WriteBuildAndRunMethod();

        _builder.WriteCodeBlockEnd();
    }

    private void WriteBuildAndRunMethod()
    {
        _builder
            .WriteLine("/// <summary>")
            .WriteLine("/// Adds the necessary model binding functionality, builds the application, and invokes the")
            .WriteLine("/// framework parsing control.")
            .WriteLine("/// <param name=\"builder\">A reference to the command line configuration object</param>")
            .WriteLine("/// <param name=\"arguments\">The arguments to parse and bind</param>")
            .WriteLine("/// <param name=\"console\">An optional reference to the console abstraction to use</param>")
            .WriteLine("/// <returns>A task that when complete, provides the integer exit code</returns>")
            .WriteLine("/// </summary>")
            .WriteGeneratedCodeAttribute()
            .WriteLine("public static async System.Threading.Tasks.Task<int> BuildAndRunAsync(")
            .Indent()
            .WriteLine("this Vertical.Cli.CommandLineBuilder builder,")
            .WriteLine("string[] arguments,")
            .WriteLine("Vertical.Cli.IO.IConsole? console = null)")
            .UnIndent()
            .WriteCodeBlockStart();

        _builder
            .WriteLine("ArgumentNullException.ThrowIfNull(builder);")
            .WriteLine("ArgumentNullException.ThrowIfNull(arguments);")
            .WriteLine();

        foreach (var entry in _modelEntries)
        {
            _builder.WriteLine($"builder.ConfigureModel<{entry.TypeSymbol}>(model => model.UseBinder({entry.ImplementationTypeName}.Bind));");
        }

        _builder
            .WriteLine()
            .WriteLine("return await builder.Build().RunAsync(arguments, console);")
            .WriteCodeBlockEnd();
    }

    private void WriteModelImplementations()
    {
        foreach(var entry in _modelEntries)
        {
            WriteModelImplementation(entry);
            _builder.EnqueueNewLine();
        }
    }

    private void WriteModelImplementation(ModelTypeInfo entry)
    {
        var baseTypeDeclaration = entry.BaseImplementationTypeName != null
            ? $" : {entry.BaseImplementationTypeName}"
            : string.Empty;

        var triviaHeading = $"// Binding implementation for model type {entry.TypeSymbol}";
        
        _builder
            .Write("// ").Write('-', triviaHeading.Length - 3).WriteLine()
            .WriteLine(triviaHeading)
            .WriteLine($"// Source type: {entry.ImplementationType}")
            .Write("// ").Write('-', triviaHeading.Length - 3).WriteLine()
            .WriteLine($"private sealed class {entry.ImplementationTypeName}{baseTypeDeclaration}")
            .WriteCodeBlockStart();

        WriteModelImplementationProperties(entry);
        WriteModelBinder(entry);

        _builder.WriteCodeBlockEnd();
    }
    
    private void WriteModelBinder(ModelTypeInfo entry)
    {
        _builder
            .WriteGeneratedCodeAttribute()
            .WriteLine($"internal static {entry.TypeSymbol} Bind(Vertical.Cli.Binding.BindingContext<{entry.TypeSymbol}> context)")
            .WriteCodeBlockStart();

        WriteModelInstanceInitializer(entry);
        WriteModelAssignableProperties(entry);

        _builder.WriteLine("return model;");
        _builder.WriteCodeBlockEnd();
    }

    private void WriteModelAssignableProperties(ModelTypeInfo entry)
    {
        foreach (var property in entry.AssignableProperties)
        {
            _builder.WriteLine($"model.{property.Name} = {_expressionHelper.GetBindingExpression(property)};");
        }
    }

    private void WriteModelInstanceInitializer(ModelTypeInfo entry)
    {
        if (entry.RequiresActivation)
        {
            _builder.WriteLine("var model = context.ActivateModelInstance();");
            return;
        }

        _builder.Write($"var model = new {entry.InitializerTypeName}");

        var (hasParams, hasProps) = (
            entry.InitializerParameters.Length > 0,
            entry.InitializerProperties.Length > 0);

        if (hasParams)
        {
            _builder.WriteLine('(').Indent();
            
            WriteMemberAssignmentList(
                entry.InitializerParameters,
                param => $"{param.Name}: {_expressionHelper.GetBindingExpression(param)}");

            _builder.UnIndent().Write(')');
        }

        if (hasProps)
        {
            _builder
                .WriteLine()
                .WriteLine('{')
                .Indent();
            
            WriteMemberAssignmentList(
                entry.InitializerProperties,
                prop => $"{prop.Name} = {_expressionHelper.GetBindingExpression(prop)}");
            
            _builder
                .WriteLine()
                .UnIndent()
                .Write('}');
        }

        if (!(hasParams || hasProps))
        {
            _builder.Write("()");
        }

        _builder.WriteLine(';');
    }

    private void WriteMemberAssignmentList<T>(
        IEnumerable<T> symbols,
        Func<T, string> assignmentExpression)
        where T : ISymbol
    {
        var next = false;

        foreach (var symbol in symbols)
        {
            if (next)
            {
                _builder.WriteLine(',');
            }
            
            _builder.Write(assignmentExpression(symbol));
            next = true;
        }
    }

    private void WriteModelImplementationProperties(ModelTypeInfo entry)
    {
        foreach (var property in entry.DeclarableProperties)
        {
            _builder.WriteModelImplementationPropertyDeclaration(property);
        }

        _builder.EnqueueNewLine(entry.DeclarableProperties.Length > 0);
    }

    private void WriteHeader()
    {
        _builder
            .WriteLine("namespace Vertical.Cli;")
            .WriteLine("#nullable enable")
            .WriteLine();
    }
}