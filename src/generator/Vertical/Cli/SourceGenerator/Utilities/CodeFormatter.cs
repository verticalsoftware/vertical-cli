using System.Text;

namespace Vertical.Cli.SourceGenerator.Utilities;

public sealed class CodeFormatter
{
    private readonly StringBuilder _buffer = new(5000);
    private int _indent;
    private bool _atLineStart = true;
    private bool _writeNewLine;
    private int _track;

    public CodeFormatter Indent() => IncrementIndent(4);

    public CodeFormatter UnIndent() => IncrementIndent(-4);

    public CodeFormatter Track()
    {
        _track = _buffer.Length;
        return this;
    }

    public bool HasChanges => _buffer.Length > _track;

    public CodeFormatter Write(char c, int repeatCount = 1)
    {
        WriteLineStart();
        while (--repeatCount >= 0)
        {
            _buffer.Append(c);
        }
        return this;
    }
    
    public CodeFormatter Write(string str)
    {
        WriteLineStart();
        _buffer.Append(str);
        return this;
    }

    public CodeFormatter WriteLine()
    {
        _buffer.AppendLine();
        _atLineStart = true;
        return this;
    }

    public CodeFormatter WriteLine(string str) => Write(str).WriteLine();

    public CodeFormatter WriteLine(char c) => Write(c).WriteLine();

    public CodeFormatter WriteCodeBlockStart() => WriteLine('{').Indent();
    
    public CodeFormatter WriteCodeBlockEnd() => UnIndent().WriteLine('}');

    public ListWriter CreateListWriter() => new(this);

    public CodeFormatter EnqueueNewLine(bool condition = true)
    {
        _writeNewLine = _writeNewLine || condition;
        return this;
    }

    public CodeFormatter DequeueNewLine()
    {
        _writeNewLine = false;
        return this;
    }

    /// <inheritdoc />
    public override string ToString() => _buffer.ToString();

    public int Length => _buffer.Length;

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

    private CodeFormatter IncrementIndent(int value)
    {
        _indent += value;
        return this;
    }
}