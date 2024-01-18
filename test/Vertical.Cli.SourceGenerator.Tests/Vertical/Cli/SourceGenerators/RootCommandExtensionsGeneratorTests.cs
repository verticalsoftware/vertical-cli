using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vertical.Cli.SourceGenerator;

namespace Vertical.Cli.SourceGenerators;

[UsesVerify]
public class RootCommandExtensionsGeneratorTests
{
    [Fact]
    public Task GeneratedSourcesVerified()
    {
        var compilation = CSharpCompilation.Create(
            assemblyName: "Test",
            syntaxTrees: new[]
            {
                CSharpSyntaxTree.ParseText(Resources.ProgramCSharpCode)
            },
            references: new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            });

        var generator = new RootCommandExtensionsGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        var result = driver.RunGenerators(compilation);

        return Verify(result);
    }
}