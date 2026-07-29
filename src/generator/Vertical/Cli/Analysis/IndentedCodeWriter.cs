using System.Text;

namespace Vertical.Cli.Analysis;

public delegate void NestedWriterAction(ref IndentedCodeWriter inner);

public delegate void NestedWriterAction<in T>(T data, ref IndentedCodeWriter inner);

public ref struct  IndentedCodeWriter
{
    public static readonly (char, char) CurlyBraces = ('{', '}');
    public static readonly (char, char) SquareBrackets = ('[', ']');
    public static readonly (char, char) AngleBrackets = ('<', '>');
    public static readonly (char, char) Parenthesis = ('(', ')');

    public IndentedCodeWriter(StringBuilder sb, int indentSpaces)
    { 
        _sb = sb;
        _indentSpaces = indentSpaces;
        _isNewLine = true;
    }
    
    private const int IndentIncrement = 3;
    private readonly int _indentSpaces;
    private readonly StringBuilder _sb;
    private bool _isNewLine;

    /// <inheritdoc />
    public override string ToString() => _sb.ToString();

    public void Write(char c)
    {
        InitializeLinePosition();
        _sb.Append(c);
        _isNewLine = false;
    }

    public void Write(string str)
    {
        InitializeLinePosition();
        _sb.Append(str);
        _isNewLine = false;
    }

    public void WriteLine()
    {
        _sb.AppendLine();
        _isNewLine = true;
    }

    public void WriteLine(char c)
    {
        Write(c);
        WriteLine();
    }

    public void WriteLine(string str)
    {
        Write(str);
        WriteLine();
    }

    public void WriteBlock((char open, char close) bookEnds, NestedWriterAction action)
    {
        Return();
        Write(bookEnds.open);
        WriteLine();
        
        var inner = new IndentedCodeWriter(_sb, _indentSpaces + IndentIncrement);
        action(ref inner);
        
        Return();
        Write(bookEnds.close);
        WriteLine();
    }
    
    public void WriteParameterList(
        (char open, char close) bookEnds,
        IEnumerable<string> parameters,
        bool returnToNewLine = true)
    {
        Write(bookEnds.open);
        WriteLine();

        var inner = Indent();
        var lineId = 0;
        
        foreach (var parameter in parameters)
        {
            if (lineId > 0)
            {
                inner.WriteLine();
                inner.Write(", ");
            }
            inner.Write(parameter);
            ++lineId;
        }

        inner.Write(bookEnds.close);

        if (returnToNewLine)
        {
            WriteLine();
        }
    }
    
    public void WriteInitializerList(
        (char open, char close) bookEnds,
        IEnumerable<string> parameters,
        bool returnToNewLine = true)
    {
        Return();
        Write(bookEnds.open);
        WriteLine();

        var inner = Indent();
        var lineId = 0;
        
        foreach (var parameter in parameters)
        {
            if (lineId > 0)
            {
                inner.WriteLine();
                inner.Write(", ");
            }
            inner.Write(parameter);
            ++lineId;
        }

        inner.WriteLine();
        Write(bookEnds.close);

        if (returnToNewLine)
        {
            WriteLine();
        }
    }

    public void Return()
    {
        if (_isNewLine) return;
        WriteLine();
    }
    
    private IndentedCodeWriter Indent() => new(_sb, _indentSpaces + IndentIncrement);

    private void InitializeLinePosition()
    {
        if (!_isNewLine) return;

        _sb.Append(' ', _indentSpaces);
        _isNewLine = false;
    }
}