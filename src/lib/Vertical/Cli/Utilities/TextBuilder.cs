using System.Text;

namespace Vertical.Cli.Utilities;

internal sealed class TextBuilder
{
    private readonly List<string> _rows = new();
    private readonly StringBuilder _buffer = new();

    internal TextBuilder Clear()
    {
        _rows.Clear();
        _buffer.Clear();
        return this;
    }

    internal void Append(string str) => _buffer.Append(str);

    internal void AppendLine() => FlushRow();

    internal void AppendLine(string str)
    {
        Append(str);
        FlushRow();
    }

    internal void AppendLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            AppendLine(line);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        FlushRow();
        return string.Join(Environment.NewLine, _rows);
    }

    /// <summary>
    /// Gets the rows in the text builder.
    /// </summary>
    public IReadOnlyList<string> Rows => _rows;

    private void FlushRow()
    {
        if (_buffer.Length == 0)
            return;
        _rows.Add(_buffer.ToString());
        _buffer.Clear();
    }
}