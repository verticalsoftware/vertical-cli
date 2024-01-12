using System.Text;
using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Help;

public sealed class HelpTextWriter
{
    private readonly TextWriter _textWriter;
    private readonly int _width;
    private readonly char[] _spaceBuffer;
    private int _x;

    public HelpTextWriter(TextWriter textWriter, int width)
    {
        Guard.IsNotNull(textWriter);
        
        _textWriter = textWriter;
        _width = width;
        _spaceBuffer = new char[width];
        Array.Fill(_spaceBuffer, ' ');
    }

    public void Flush() => _textWriter.Flush();

    public void Write(char c)
    {
        _textWriter.Write(c);
        ++_x;
    }

    public void Write(ReadOnlySpan<char> span, (int Left, int Right)? margin = null)
    {
        if (margin.HasValue)
        {
            WriteBlock(span, margin.Value);
            return;
        }
        
        _textWriter.Write(span);
        _x += span.Length;
    }

    public void WriteLine(int count = 1)
    {
        for (var c = 0; c < count; c++)
        {
            _textWriter.WriteLine();
        }

        _x = 0;
    }

    public void WriteLine(ReadOnlySpan<char> span, (int Left, int Right)? margin = null)
    {
        Write(span, margin);
        WriteLine();
    }

    public void WriteSpace()
    {
        _textWriter.Write(' ');
        ++_x;
    }

    public void Indent(int margin) => WriteNewLineLeftMargin(margin);

    public void WriteLineIfNotAtLineOrigin()
    {
        if (_x == 0) return;
        WriteLine();
    }
    
    private void WriteBlock(ReadOnlySpan<char> span, (int Left, int Right) margin)
    {
        if (span.Length == 0)
            return;

        // Split to new lines
        while (span.Length > 0)
        {
            var ptr = 0;
            
            // Stop at new line
            while (ptr < span.Length && span[ptr] is not ('\r' or '\n'))
                ++ptr;

            // Write any segment up to this
            if (ptr > 0)
            {
                WriteNewLineLeftMargin(margin.Left);
                WriteBlockSegment(span[..ptr], margin);       
            }

            if (ptr == span.Length)
                return;
            
            WriteLine();
            
            // Advance past new line characters
            if (span[ptr] == '\r')
                ++ptr;

            if (ptr < span.Length && span[ptr] == '\n')
                ++ptr;

            if (ptr == span.Length)
                return;

            span = span[ptr..];
        }
    }

    private void WriteBlockSegment(ReadOnlySpan<char> span, (int Left, int Right) margin)
    {
        if (span.Length == 0)
            return;
        
        var blockWidth = _width - margin.Left - margin.Right - 1;
        
        while (span.Length > 0)
        {
            WriteNewLineLeftMargin(margin.Left);

            if (span.Length <= blockWidth)
            {
                // No wrap needed
                Write(span);
                return;
            }

            var ptr = blockWidth;
            
            // Backtrack to whitespace break
            while (ptr > 0 && !char.IsWhiteSpace(span[ptr]))
                --ptr;

            if (ptr == 0)
            {
                // Can't break
                Write(span);
                return;
            }
            
            var segment = span[..ptr];
            Write(segment);

            while (ptr < span.Length && char.IsWhiteSpace(span[ptr]))
                ++ptr;

            if (ptr == span.Length)
                return;

            WriteLine();
            span = span[ptr..];
        }
    }


    private void WriteSpaces(int count)
    {
        if (count < 1) return;
        _textWriter.Write(_spaceBuffer, 0, count);
        _x += count;
    }

    private void WriteNewLineLeftMargin(int margin)
    {
        WriteSpaces(margin - _x);
    }

    private string Content
    {
        get
        {
            Flush();
            var streamWriter = (StreamWriter)_textWriter;
            var source = (MemoryStream)streamWriter.BaseStream;
            var bytes = source.ToArray();
            return Encoding.UTF8.GetString(bytes);
        }
    }
}