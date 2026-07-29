using System.Text;
using Vertical.Cli.IO;

namespace Vertical.Cli.ScenarioTests.Common;

public class TestConsole : IConsole
{
    private readonly StringWriter _writer = new(new StringBuilder());

    /// <inheritdoc />
    public TextReader In => throw new NotSupportedException();

    /// <inheritdoc />
    public TextWriter Out => _writer;

    /// <inheritdoc />
    public bool IsOutputRedirected => true;

    /// <inheritdoc />
    public int DisplayWidth => 120;

    /// <inheritdoc />
    public override string ToString()
    {
        var content = _writer.ToString();

        return content.ReplaceLineEndings("\n");
    }
}