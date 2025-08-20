namespace Vertical.Cli.Help;

/// <summary>
/// Represents an output help writer that colorizes elements of the output.
/// </summary>
public sealed class ColoredOutputHelpWriter : IHelpTextWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColoredOutputHelpWriter"/> class.
    /// </summary>
    /// <param name="textWriter">The text writer where final output will be written.</param>
    /// <param name="palette">A dictionary of ansi foreground colors that are associated to the
    /// renderable help elements.</param>
    public ColoredOutputHelpWriter(TextWriter textWriter, Dictionary<HelpElementKind, byte>? palette = null)
    {
        TextWriter = textWriter;
        _cachedFormatStrings = BuildCache(palette ?? DefaultPalette);
    }


    private const int BaseCode = 30;
    private const string DefaultFormatString = "\x1b[0m";

    /// <summary>
    /// Gets the default palette.
    /// </summary>
    public static readonly Dictionary<HelpElementKind, byte> DefaultPalette = new()
    {
        [HelpElementKind.ListItemParameterSyntax] = 106,
        [HelpElementKind.CommandDescription] = 79,
        [HelpElementKind.ListItemDescription] = 79,
        [HelpElementKind.CommandUsageName] = 215,
        [HelpElementKind.ListItemIdentifier] = 215
    };

    /// <summary>
    /// Gets the underlying text writer.
    /// </summary>
    public TextWriter TextWriter { get; }
    
    private readonly Dictionary<HelpElementKind, string> _cachedFormatStrings;

    /// <inheritdoc />
    public void WriteElement(HelpElementKind elementKind, string value) => WriteElement(elementKind, value.AsSpan());

    /// <inheritdoc />
    public void WriteElement(HelpElementKind elementKind, ReadOnlySpan<char> valueSpan)
    {
        if (!_cachedFormatStrings.TryGetValue(elementKind, out var formatString))
        {
            TextWriter.Write(valueSpan);
            return;
        }

        try
        {
            TextWriter.Write(formatString);
            TextWriter.Write(valueSpan);
        }
        finally
        {
            TextWriter.Write(DefaultFormatString);
        }
    }

    /// <inheritdoc />
    public void WriteLine(int count = 1)
    {
        while (--count >= 0)
        {
            TextWriter.WriteLine();
        }
    }

    /// <inheritdoc />
    public void WriteWhiteSpace(int count)
    {
        while (--count >= 0)
        {
            TextWriter.Write(' ');
        }
    }

    private static Dictionary<HelpElementKind, string> BuildCache(Dictionary<HelpElementKind, byte> palette)
    {
        return palette.ToDictionary(kv => kv.Key, kv => $"\x1b[38;5;{BaseCode + kv.Value}m");
    }
}