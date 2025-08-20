using System.Text;

namespace Vertical.Cli.SourceGenerator;

public sealed class CodeFormattedStringBuilder
{
    private readonly StringBuilder _buffer = new(5000);
    private int _indent;
    private bool _atLineStart = true;
    private bool _writeNewLine = false;

    public CodeFormattedStringBuilder Indent() => IncrementIndent(4);

    public CodeFormattedStringBuilder UnIndent() => IncrementIndent(-4);

    public CodeFormattedStringBuilder Write(char c, int repeatCount = 1)
    {
        WriteLineStart();
        while (--repeatCount >= 0)
        {
            _buffer.Append(c);
        }
        return this;
    }
    
    public CodeFormattedStringBuilder Write(string str)
    {
        WriteLineStart();
        _buffer.Append(str);
        return this;
    }

    public CodeFormattedStringBuilder WriteLine()
    {
        _buffer.AppendLine();
        _atLineStart = true;
        return this;
    }

    public CodeFormattedStringBuilder WriteCsvLineList<T>(
        IEnumerable<T> items,
        Func<T, string> getLineContent,
        (char, char)? enclosingTokens = null)
    {
        if (enclosingTokens.HasValue)
        {
            WriteLine(enclosingTokens.Value.Item1);
            Indent();
        }

        var id = 0;

        foreach (var item in items)
        {
            if (id++ > 0)
            {
                Write(',').WriteLine();
            }

            Write(getLineContent(item));
        }

        if (id > 0)
        {
            WriteLine();
        }

        if (enclosingTokens.HasValue)
        {
            UnIndent();
            Write(enclosingTokens.Value.Item2);
        }

        return this;
    }

    public CodeFormattedStringBuilder WriteLine(string str) => Write(str).WriteLine();

    public CodeFormattedStringBuilder WriteLine(char c) => Write(c).WriteLine();

    public CodeFormattedStringBuilder WriteCodeBlockStart() => WriteLine('{').Indent();
    
    public CodeFormattedStringBuilder WriteCodeBlockEnd() => UnIndent().WriteLine('}');

    public CodeFormattedStringBuilder EnqueueNewLine(bool condition = true)
    {
        _writeNewLine = _writeNewLine || condition;
        return this;
    }

    public CodeFormattedStringBuilder DequeueNewLine()
    {
        _writeNewLine = false;
        return this;
    }

    /// <inheritdoc />
    public override string ToString() => _buffer.ToString();

    private void WriteLineStart()
    {
        if (_writeNewLine)
        {
            _writeNewLine = false;
            _buffer.AppendLine();
        }
        
        if (!_atLineStart)
            return;

        for (var c = 0; c < _indent; c++)
        {
            _buffer.Append(' ');
        }

        _atLineStart = false;
    }

    private CodeFormattedStringBuilder IncrementIndent(int value)
    {
        _indent += value;
        return this;
    }
}