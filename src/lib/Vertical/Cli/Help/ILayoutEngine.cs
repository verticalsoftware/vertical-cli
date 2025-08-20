namespace Vertical.Cli.Help;

/// <summary>
/// Represents a component of the help system that aligns, trims, and wraps content
/// from the help provider before it is rendered by the help text writer.
/// </summary>
public interface ILayoutEngine
{
    /// <summary>
    /// Writes a section title.
    /// </summary>
    /// <param name="sectionTitle">The section title</param>
    void WriteSectionTitle(string sectionTitle);

    /// <summary>
    /// Writes a section ending.
    /// </summary>
    void WriteSectionEnd();

    /// <summary>
    /// Writes a usage clause with a single part.
    /// </summary>
    /// <param name="commandName">The command name</param>
    void WriteUsageClause(string commandName);

    /// <summary>
    /// Writes a usage clause with a single token.
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="token">A usage token</param>
    void WriteUsageClause(string commandName, UsageToken token);

    /// <summary>
    /// Writes a usage clause with two tokens.
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="firstToken">The first token</param>
    /// <param name="secondToken">The second token</param>
    void WriteUsageClause(string commandName, UsageToken firstToken, UsageToken secondToken);

    /// <summary>
    /// Writes the command description section.
    /// </summary>
    /// <param name="sectionTitle">The section title</param>
    /// <param name="description">The command description</param>
    void WriteCommandDescriptionSection(string sectionTitle, string description);

    /// <summary>
    /// Writes an identifier list.
    /// </summary>
    /// <param name="listItems">List items to display.</param>
    void WriteListItems(HelpListItem[] listItems);

    /// <summary>
    /// Gets the help text writer.
    /// </summary>
    IHelpTextWriter HelpWriter { get; }

    /// <summary>
    /// Gets the constrained column width.
    /// </summary>
    int ColumnWidth { get; }

    /// <summary>
    /// Gets the indentation width.
    /// </summary>
    int IndentationSpaces { get; }

    /// <summary>
    /// Writes one or more newline characters.
    /// </summary>
    /// <param name="count">Number of newline characters to write.</param>
    void WriteLine(int count = 1);

    /// <summary>
    /// Writes an overflowing element, e.g. one that will be automatically wrapped by the display
    /// and not by the engine.
    /// </summary>
    /// <param name="elementKind">Kind of element to write.</param>
    /// <param name="value">Element content.</param>
    void WriteOverflowingElement(HelpElementKind elementKind, string value);

    /// <summary>
    /// Writes a paragraph block of content.
    /// </summary>
    /// <param name="elementKind">The element kind.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="indentLevel">The level of indentation.</param>
    void WriteParagraph(HelpElementKind elementKind, string content, int indentLevel = 1);

    /// <summary>
    /// Writes whitespace characters that represent an indentation.
    /// </summary>
    void WriteIndentation();
}