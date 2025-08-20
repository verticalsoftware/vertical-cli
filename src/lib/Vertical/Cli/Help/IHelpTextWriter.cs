namespace Vertical.Cli.Help;

/// <summary>
/// Represents a component of the help system that writes content elements to the
/// output text writer.
/// </summary>
public interface IHelpTextWriter
{
    /// <summary>
    /// Writes a help element.
    /// </summary>
    /// <param name="elementKind">The element kind.</param>
    /// <param name="value">The value to write.</param>
    void WriteElement(HelpElementKind elementKind, string value);

    /// <summary>
    /// Writes a help element.
    /// </summary>
    /// <param name="elementKind">The element kind.</param>
    /// <param name="valueSpan">The value span to write.</param>
    void WriteElement(HelpElementKind elementKind, ReadOnlySpan<char> valueSpan);

    /// <summary>
    /// Writes a blank line.
    /// </summary>
    /// <param name="count">The number of times to repeat the operation.</param>
    void WriteLine(int count = 1);

    /// <summary>
    /// Writes the given number of spaces.
    /// </summary>
    /// <param name="count">The number of whitespace characters to write.</param>
    void WriteWhiteSpace(int count);
}