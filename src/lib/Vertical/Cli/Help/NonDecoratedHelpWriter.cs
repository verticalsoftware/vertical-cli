namespace Vertical.Cli.Help;

/// <summary>
/// Represents a help-specific text writer that does not apply any extra formatting
/// to output content.
/// </summary>
public sealed class NonDecoratedHelpWriter : IHelpTextWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonDecoratedHelpWriter"/> class.
    /// </summary>
    /// <param name="textWriter">The underlying text writer.</param>
    public NonDecoratedHelpWriter(TextWriter textWriter)
    {
        TextWriter = textWriter;
    }

    /// <summary>
    /// Gets the text writer this instance will write content to.
    /// </summary>
    public TextWriter TextWriter { get; }

    /// <summary>
    /// Writes a help element.
    /// </summary>
    /// <param name="elementKind">The element kind.</param>
    /// <param name="value">The value to write.</param>
    public void WriteElement(HelpElementKind elementKind, string value)
    {
        TextWriter.Write(value);
    }

    /// <summary>
    /// Writes a help element.
    /// </summary>
    /// <param name="elementKind">The element kind.</param>
    /// <param name="valueSpan">The value span to write.</param>
    public void WriteElement(HelpElementKind elementKind, ReadOnlySpan<char> valueSpan)
    {
        TextWriter.Write(valueSpan);
    }

    /// <summary>
    /// Writes a blank line.
    /// </summary>
    /// <param name="count">The number of times to repeat the operation.</param>
    public void WriteLine(int count = 1)
    {
        while (--count >= 0)
        {
            TextWriter.WriteLine();
        }
    }

    /// <summary>
    /// Writes the given number of spaces.
    /// </summary>
    /// <param name="count">The number of whitespace characters to write.</param>
    public void WriteWhiteSpace(int count)
    {
        while (--count >= 0)
        {
            TextWriter.Write(' ');
        }
    }
}