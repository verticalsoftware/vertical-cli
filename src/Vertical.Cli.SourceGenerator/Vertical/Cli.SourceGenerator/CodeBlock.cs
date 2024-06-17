using System.Text;
using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public delegate void Writer(ref CodeBlock code);

public ref struct CodeBlock
{
    private readonly StringBuilder _sb;
    private readonly int _indent;
    private bool _origin = true;

    public CodeBlock(StringBuilder sb, int indent = 0)
    {
        _sb = sb;
        _indent = indent;
    }

    public void Append(char c)
    {
        PrepareIndentation();
        _sb.Append(c);
        _origin = false;
    }
    
    public void Append(string str)
    {
        PrepareIndentation();
        _sb.Append(str);
        _origin = false;
    }

    public void AppendSp(string str)
    {
        Append(str);
        _sb.Append(' ');
    }

    public void AppendIf(string str, bool condition)
    {
        if (!condition) return;
        Append(str);
    }

    public void AppendLine(string str = "")
    {
        if (str.Length > 0)
            Append(str);
        _sb.AppendLine();
        _origin = true;
    }

    public void AppendUnIndentedLine(string str)
    {
        _sb.AppendLine(str);
        _origin = true;
    }

    public void AppendListItem(Separator separator, string str)
    {
        separator.Next();
         
        if (separator.Iteration > 0)
        {
            Append(separator.Style.Token);
            if (separator.Style.ItemPerLine)
                AppendLine();
        }
        Append(str);
    }

    public void AppendBlock(BlockStyle style, Writer writer)
    {
        if (style.FinalizeOuterBlock)
            FinalizeLine();

        if (style.OpeningChar.HasValue)
        {
            Append(style.OpeningChar.Value);
            if (style.NewLineAfterOpeningChar)
                AppendLine();
        }

        var inner = new CodeBlock(_sb, _indent + 1);
        writer(ref inner);
        
        if (style.FinalizeInnerBlock)
            inner.FinalizeLine();

        if (!style.ClosingChar.HasValue) 
            return;

        _origin = inner._origin;
        Append(style.ClosingChar.Value);
        if (style.NewLineAfterClosingChar)
            AppendLine();
    }

    /// <inheritdoc />
    public override string ToString() => _sb.ToString();

    private void FinalizeLine()
    {
        if (_origin) return;
        AppendLine();
    }

    private void PrepareIndentation()
    {
        if (!_origin) return;
        WriteIndent();
        _origin = false;
    }

    private void WriteIndent()
    {
        for (var c = 0; c < _indent; c++)
            _sb.Append("\t");
    }
}