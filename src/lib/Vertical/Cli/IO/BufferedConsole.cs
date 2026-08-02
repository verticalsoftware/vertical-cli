namespace Vertical.Cli.IO;

internal sealed class BufferedConsole : IConsole, IDisposable
{
    private readonly StringWriter _textWriter = new();
    private readonly IConsole _underlyingConsole;

    public BufferedConsole(IConsole underlyingConsole)
    {
        _underlyingConsole = underlyingConsole;
    }
    
    public void Dispose() => _textWriter.Dispose();


    /// <inheritdoc />
    public TextReader In => throw new NotSupportedException();

    /// <inheritdoc />
    public TextWriter Out => _textWriter;

    /// <inheritdoc />
    public bool IsOutputRedirected => _underlyingConsole.IsOutputRedirected;

    /// <inheritdoc />
    public int DisplayWidth => _underlyingConsole.DisplayWidth;

    /// <summary>
    /// Flushes output to the underlying console.
    /// </summary>
    public void Flush()
    {
        var text = _textWriter.ToString();
        _underlyingConsole.Out.Write(text);        
    }
}