using System.Text;

namespace Vertical.Cli.Utilities;

internal readonly struct CsvStringBuilder
{
    private readonly string _separator;
    private readonly StringBuilder _sb;

    public CsvStringBuilder() : this(", ")
    {
    }
    
    public CsvStringBuilder(string separator)
    {
        _sb = new();
        _separator = separator;
    }

    public void Add(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return;

        if (_sb.Length > 0) _sb.Append(_separator);
        _sb.Append(str);
    }

    /// <inheritdoc />
    public override string ToString() => _sb.ToString();
}