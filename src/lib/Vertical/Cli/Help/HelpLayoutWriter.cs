using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

internal sealed class HelpLayoutWriter(TextWriter textWriter, HelpFormattingOptions formattingOptions, int width)
{
    private string enqueued = string.Empty;
    private int linePosition = 0;

    public HelpLayoutWriter EnqueueSpace() => Enqueue(" ");
    
    public HelpLayoutWriter Enqueue(string str)
    {
        enqueued = str;
        return this;
    }
    
    public HelpLayoutWriter EnqueueBreak()
    {
        enqueued = Environment.NewLine;
        return this;
    }

    public HelpLayoutWriter Indent(int level = 1) => IndentSpaces(level * formattingOptions.IndentSpaces);

    public HelpLayoutWriter IndentSpaces(int count)
    {
        Dequeue();
        for (var c = 0; c < count; c++)
        {
            textWriter.Write(' ');
        }

        enqueued = string.Empty;
        linePosition += count;
        return this;
    }

    public HelpLayoutWriter IndentTo(int anchor)
    {
        while (linePosition < anchor)
        {
            textWriter.Write(' ');
            ++linePosition;
        }

        return this;
    }

    public HelpLayoutWriter Write(HelpElement element, string? str)
    {
        if (str == null)
            return this;

        Dequeue();
        
        textWriter.Write(formattingOptions.OutputFormatter(element, str));
        linePosition += str.Length;
        return this;
    }

    public HelpLayoutWriter WriteAnchored2(HelpElement element, string? str)
    {
        if (str == null)
            return this;
        
        Dequeue();

        var anchor = linePosition;
        var span = str.AsSpan();
        var anchoredWidth = width - anchor;

        while (true)
        {
            var split = span.SplitAtWhiteSpace(anchoredWidth);
            textWriter.Write(formattingOptions.OutputFormatter(element, split.Left.ToString()));

            if (split.Right.Length == 0)
                return this;

            WriteLine();
            IndentSpaces(anchor);
            span = split.Right;
        }
    }
    
    public HelpLayoutWriter WriteAnchored(HelpElement element, string? str)
    {
        if (str == null)
            return this;

        Dequeue();

        var anchor = linePosition;
        var anchoredWidth = width - anchor;
        var span = str.AsSpan();

        while (true)
        {
            var outer = span.SplitAtLineBreak();
            var inner = outer.Left;
            
            while (true)
            {
                var split = inner.SplitAtWhiteSpace(anchoredWidth);
                textWriter.Write(formattingOptions.OutputFormatter(element, split.Left.ToString()));

                if (split.Right.Length == 0)
                    break;

                WriteLine();
                IndentSpaces(anchor);
                inner = split.Right;
            }

            if (outer.Right.Length == 0)
                break;

            WriteLine();
            IndentSpaces(anchor);
            span = outer.Right;
        }

        return this;
    }

    public HelpLayoutWriter WriteLine()
    {
        textWriter.WriteLine();
        enqueued = string.Empty;
        linePosition = 0;
        return this;
    }

    public HelpLayoutWriter WriteLine(HelpElement element, string? str)
    {
        if (str == null)
            return this;

        Write(element, str);
        return WriteLine();
    }

    private void Dequeue()
    {
        if (enqueued.Length == 0)
            return;
        
        textWriter.Write(enqueued);
        linePosition += enqueued.Length;
        enqueued = string.Empty;
    }
}