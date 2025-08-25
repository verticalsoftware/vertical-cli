using Microsoft.CodeAnalysis;
using Vertical.Cli.SourceGenerator.ModelControllers;
using Vertical.Cli.SourceGenerator.Utilities;

namespace Vertical.Cli.SourceGenerator;

public sealed class CodeGenerator
{
    public CodeGenerator(HashSet<ITypeSymbol> typeSymbols)
    {
        var expressionHelper = new BindingExpressionHelper("context");
        
        _controllers = typeSymbols
            .Select(typeSymbol => ModelController.Create(typeSymbol, expressionHelper))
            .Where(controller => controller is not null)
            .Cast<IModelController>()
            .ToArray();
    }

    private readonly CodeFormatter _builder = new();
    private readonly IModelController[] _controllers;

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

        foreach (var controller in _controllers)
        {
            _builder.WriteLine($"builder.ConfigureModel<{controller.TypeSymbol}>(model => model.UseBinder({controller.ImplementationTypeName}.Bind));");
        }

        _builder
            .WriteLine()
            .WriteLine("return await builder.Build().RunAsync(arguments, console);")
            .WriteCodeBlockEnd();
    }

    private void WriteModelImplementations()
    {
        foreach(var controller in _controllers)
        {
            controller.WriteImplementation(_builder);
            _builder.EnqueueNewLine();
        }
    }
    
    private void WriteHeader()
    {
        _builder
            .WriteLine("namespace Vertical.Cli;")
            .WriteLine("#nullable enable")
            .WriteLine();
    }
}