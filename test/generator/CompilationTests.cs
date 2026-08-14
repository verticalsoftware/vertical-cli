using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Vertical.Cli.Analysis;
using Vertical.Cli.Binding;

namespace Vertical.Cli.SourceGenerator.Tests;

public class CompilationTests
{
    [Fact]
    public void Generated_Compilable_Code()
    {
        var references =
            ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile(typeof(GeneratedBindingAttribute).Assembly.Location))
            .ToArray();

        var cancellationToken = TestContext.Current.CancellationToken;

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [CSharpSyntaxTree.ParseText(TestSource.Value, cancellationToken: cancellationToken)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        
        var generator = new CommandLineApplicationExtensionsGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        var generatorDriver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var updatedCompilation,
            out _,
            cancellationToken);

        var result = generatorDriver.GetRunResult();
        
        updatedCompilation.GetDiagnostics(cancellationToken).ShouldBeEmpty();
        result.Diagnostics.ShouldBeEmpty();
    }
}