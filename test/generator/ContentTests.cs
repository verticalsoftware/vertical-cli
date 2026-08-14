using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vertical.Cli.Analysis;
using Vertical.Cli.Binding;

namespace Vertical.Cli.SourceGenerator.Tests;

public class ContentTests
{
    [Fact]
    public Task Generated_Expected_Code()
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
            out _,
            out _,
            cancellationToken);

        var result = generatorDriver.GetRunResult();
        var generatedSource = result
            .Results[0]
            .GeneratedSources[0]
            .SourceText
            .ToString();

        var verifySettings = new VerifySettings();
        verifySettings.AddScrubber(sb =>
        {
            var classIdReplacedString = Regex.Replace(
                sb.ToString(),
                "[A-F0-9]{8}",
                "<ClassId>");

            sb.Clear();
            sb.Append(classIdReplacedString);
        });
        
        return Verify(generatedSource, settings: verifySettings);
    }   
}