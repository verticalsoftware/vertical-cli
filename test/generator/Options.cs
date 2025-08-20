using System.Collections.Immutable;
using Vertical.Cli.Binding;

namespace GeneratorTestApp;


[GeneratedBinding]
public class OptionsClass
{
    public string? Property { get; set; }

    public TextReader InputStream { get; set; } =  null!;
}