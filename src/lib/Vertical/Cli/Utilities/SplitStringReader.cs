namespace Vertical.Cli.Utilities;

public ref struct SplitStringReader
{
    private readonly int _splitLength;
    private ReadOnlySpan<char> _span;
    
    public SplitStringReader(string str, int splitLength)
    {
        _splitLength = splitLength;
        _span = str.AsSpan().Trim();
    }

    public bool TryReadLine(out ReadOnlySpan<char> lineSpan)
    {
        lineSpan = [];

        // Past end
        if (_span.Length == 0) return false;

        var maxLength = Math.Min(_splitLength, _span.Length);
        var span = _span[..maxLength];
        var lineBreak = span.IndexOfAny(['\r', '\n']);

        switch (lineBreak)
        {
            case 0:
                TrimLeadingLineBreak();
                AdvanceToNextLineReadPosition(0);
                return true;
            
            case > -1:
                lineSpan = span[..lineBreak];
                TrimLeadingLineBreak(lineBreak);
                AdvanceToNextLineReadPosition(0);
                return true;
            
            case -1 when _span.Length <= _splitLength:
                lineSpan = _span;
                _span = [];
                return true;
        }

        for (var position = span.Length - 1; position >= 0; position--)
        {
            var c = span[position];
            
            if (!char.IsWhiteSpace(c))
                continue;
            
            // position = the whitespace char
            lineSpan = span[..position];
            AdvanceToNextLineReadPosition(position);
            return true;
        }
        
        
        // If here then the line is not splittable to a non-breaking whitespace char
        maxLength = Math.Min(_splitLength, span.Length);
        lineSpan = span[..maxLength];
        AdvanceToNextLineReadPosition(maxLength);
        return true;
    }

    private void AdvanceToNextLineReadPosition(int position)
    {
        while (position < _span.Length)
        {
            var c = _span[position];

            // Continue past whitespace that isn't line breaks
            if (c is '\r' or '\n' || !char.IsWhiteSpace(c))
                break;

            ++position;
        }

        _span = _span[position..];
    }

    private void TrimLeadingLineBreak(int position = 0)
    {
        if (position < _span.Length && _span[position] == '\r') ++position;
        if (position < _splitLength && _span[position] == '\n') ++position;

        _span = _span[position..];
    }
}