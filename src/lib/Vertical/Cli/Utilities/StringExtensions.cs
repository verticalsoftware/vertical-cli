using System.Text;

namespace Vertical.Cli.Utilities;

internal static class StringExtensions
{
    private static readonly StringBuilder ReusableBuffer = new();
    
    internal readonly ref struct SplitSpan
    {
        public SplitSpan(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
        {
            Left = left;
            Right = right;
        }

        public ReadOnlySpan<char> Left { get; }

        public ReadOnlySpan<char> Right { get; }
    }

    internal static SplitSpan SplitAtLineBreak(this ReadOnlySpan<char> span)
    {
        for (var c = 0; c < span.Length; c++)
        {
            if (span[c] != '\n')
                continue;

            return new SplitSpan(span[..c], span[(c+1)..]);
        }

        return new SplitSpan(span, []);
    }

    internal static SplitSpan SplitAtWhiteSpace(this ReadOnlySpan<char> span, int width)
    {
        if (span.Length < width)
        {
            return new SplitSpan(span, ReadOnlySpan<char>.Empty);
        }
        
        var wsSpan = GetWhiteSpaceSpan(span, width);

        return wsSpan switch
        {
            // Full split
            { left: > -1, right: > -1 } => new SplitSpan(span[..wsSpan.left], span[wsSpan.right..]),
            { left: > -1, right: -1 } => new SplitSpan(span[..wsSpan.left], ReadOnlySpan<char>.Empty),
            _ => new SplitSpan(span[..width], span[width..])
        };
    }

    internal static string SnakeCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;
        
        ReusableBuffer.Clear();
        ReusableBuffer.Append(str[0]);

        foreach (var ch in str.Skip(1))
        {
            if (char.IsUpper(ch))
            {
                ReusableBuffer.Append('_');
                ReusableBuffer.Append(ch);
                continue;
            }

            ReusableBuffer.Append(char.ToUpper(ch));
        }

        return ReusableBuffer.ToString();
    }

    private static (int left, int right) GetWhiteSpaceSpan(ReadOnlySpan<char> span, int index)
    {
        var (left, right) = (-1, -1);
        var len = span.Length;
        var c = Math.Min(index, len-1);

        for (; c >= 0; c--)
        {
            if (!IsWhiteSpace(span[c]))
                continue;

            left = c;

            for (; left > 0;)
            {
                if (!IsWhiteSpace(span[left - 1]))
                {
                    c = -1;
                    break;
                }

                --left;
            }
        }

        if (left == -1)
        {
            return (left, right);
        }

        right = left;

        for (; right < len - 1;)
        {
            if (!IsWhiteSpace(span[right]))
                break;

            ++right;
        }

        return right < len - 1
            ? (left, right)
            : (left, -1);
    }

    private static bool IsWhiteSpace(char c) => c == ' ';
}