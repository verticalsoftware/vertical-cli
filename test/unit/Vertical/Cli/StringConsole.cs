using System.Text;
using NSubstitute;
using Vertical.Cli.IO;

namespace Vertical.Cli;

public class StringConsole : IConsole
{
    private readonly StringWriter _writer = new();

    /// <inheritdoc />
    public TextReader In => Substitute.For<TextReader>();

    /// <inheritdoc />
    public TextWriter Out => _writer;

    /// <inheritdoc />
    public int DisplayWidth => 120;

    /// <inheritdoc />
    public bool ErrorMode { get; set; }

    /// <inheritdoc />
    public void HandleCancelEvent(Action handleCancel)
    {
    }

    /// <inheritdoc />
    public void Write(string str)
    {
        _writer.Write(str);
    }

    /// <inheritdoc />
    public void WriteLine()
    {
        _writer.WriteLine();
    }

    /// <inheritdoc />
    public void WriteLine(string str)
    {
        _writer.WriteLine(str);
    }

    /// <inheritdoc />
    public void WriteErrorLine(string str)
    {
        _writer.WriteLine(str);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        _writer.Flush();
        return _writer.ToString();
    }
}