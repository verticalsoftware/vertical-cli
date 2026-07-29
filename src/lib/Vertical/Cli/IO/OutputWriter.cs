namespace Vertical.Cli.IO;

/// <summary>
/// Provides operations to write to the output abstraction.
/// </summary>
public sealed class OutputWriter
{
    private static readonly string WhiteSpace80 = new(' ', 80);
    private readonly TextWriter _writer;
    private readonly IConsole _console;
    private readonly OutputFormatter _formatter;
    private readonly int _displayWidth;
    private DisplayElement _currentElement = DisplayElement.Default;

    internal OutputWriter(IConsole console, OutputFormatter formatter)
    {
        _writer = console.Out;
        _displayWidth = ResolveDisplayWidth(console);
        _console = console;
        _formatter = formatter;
    }

    /// <summary>
    /// Gets the display width.
    /// </summary>
    public int DisplayWidth => _displayWidth;

    /// <summary>
    /// Gets whether any text has been written since the last new line character
    /// was written.
    /// </summary>
    public bool IsNewLine { get; private set; } = true;

    /// <summary>
    /// Sets the display element.
    /// </summary>
    /// <param name="element">The display element to set.</param>
    public void SetDisplayElement(DisplayElement element) => TrySetDisplayElement(element);

    /// <summary>
    /// Writes a character.
    /// </summary>
    /// <param name="c">The character to write.</param>
    /// <param name="element">The display element or <c>null</c> to use the current style.</param>
    public void Write(char c, DisplayElement? element = null)
    {
        TrySetDisplayElement(element);
        _writer.Write(c);
        IsNewLine = false;
    }

    /// <summary>
    /// Writes a character span.
    /// </summary>
    /// <param name="span">The span to write.</param>
    /// <param name="element">The display element or <c>null</c> to use the current style.</param>
    public void Write(ReadOnlySpan<char> span, DisplayElement? element = null)
    {
        TrySetDisplayElement(element);
        _writer.Write(span);
        IsNewLine = false;
    }

    /// <summary>
    /// Writes a string.
    /// </summary>
    /// <param name="str">The string to write.</param>
    /// <param name="element">The display element or <c>null</c> to use the current style.</param>
    public void Write(string str, DisplayElement? element = null)
    {
        TrySetDisplayElement(element);
        _writer.Write(str);
        IsNewLine = false;
    }

    /// <summary>
    /// Writes whitespace characters.
    /// </summary>
    /// <param name="count">The number of whitespace characters to write.</param>
    public void WriteWhiteSpace(int count = 1)
    {
        if ((count = Math.Max(0, count)) == 0)
            return;
        
        switch (count)
        {
            case 1:
                _writer.Write(' ');
                break;
            
            case < 80:
                _writer.Write(WhiteSpace80.AsSpan(0, count));
                break;
            
            default:
                while (count > WhiteSpace80.Length)
                {
                    _writer.Write(WhiteSpace80);
                    count -= WhiteSpace80.Length;
                }
                _writer.Write(WhiteSpace80.AsSpan(0, count));
                break;
        }

        IsNewLine = IsNewLine && count % _displayWidth == 0;
    }

    /// <summary>
    /// Writes a span of characters followed by a newline.
    /// </summary>
    /// <param name="span">The span to write.</param>
    /// <param name="element">The display element or <c>null</c> to use the current style.</param>
    public void WriteLine(ReadOnlySpan<char> span, DisplayElement? element = null)
    {
        Write(span, element);
        WriteLine();
    }
    
    /// <summary>
    /// Writes a span of characters followed by a newline.
    /// </summary>
    /// <param name="str">The string to write.</param>
    /// <param name="element">The display element or <c>null</c> to use the current style.</param>
    public void WriteLine(string str, DisplayElement? element = null)
    {
        Write(str, element);
        WriteLine();
    }

    /// <summary>
    /// Writes a newline character.
    /// </summary>
    public void WriteLine()
    {
        _writer.Write('\n');
        IsNewLine = true;
    }

    /// <summary>
    /// Writes a new line if characters were written on the current list.
    /// </summary>
    public void Return()
    {
        if (IsNewLine) return;
        WriteLine();
    }
    
    private void TrySetDisplayElement(DisplayElement? element)
    {
        // Don't writer control codes to files
        if (_console.IsOutputRedirected)
            return;
        
        if (element is null || element == _currentElement)
            return;
        
        _formatter.WriteControlSequence(_writer, element.Value);
        _currentElement = element.Value;
    }

    private static int ResolveDisplayWidth(IConsole console)
    {
        try
        {
            return console.DisplayWidth;
        }
        catch
        {
            return 80;
        }
    }
}