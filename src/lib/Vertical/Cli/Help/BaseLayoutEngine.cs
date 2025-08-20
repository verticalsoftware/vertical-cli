namespace Vertical.Cli.Help;

/// <summary>
/// Represents an object that formats, aligns, and trims content to be printed by a
/// help text writer.
/// </summary>
public abstract class BaseLayoutEngine : ILayoutEngine
{
    /// <summary>
    /// Initializes a new instance of the abstract <see cref="BaseLayoutEngine"/> class.
    /// </summary>
    /// <param name="writer">The help text writer to output content to.</param>
    /// <param name="columnWidth">The number of characters that should be printed in a single row without causing
    /// wrapping..</param>
    /// <param name="indentationSpaces">The number of character printed to indicate indentation.</param>
    protected BaseLayoutEngine(
        IHelpTextWriter writer, 
        int columnWidth,
        int indentationSpaces = DefaultIndentationWidth)
    {
        HelpWriter = writer;
        ColumnWidth = Math.Max(MinimumWidth, columnWidth);
        IndentationSpaces = indentationSpaces;
        
        ArgumentOutOfRangeException.ThrowIfLessThan(columnWidth, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(indentationSpaces, ColumnWidth);
    }

    /// <summary>
    /// Defines the default minimum width.
    /// </summary>
    public const int MinimumWidth = 40;
    
    /// <summary>
    /// Defines the default indentation width.
    /// </summary>
    public const int DefaultIndentationWidth = 2;
    
    /// <summary>
    /// Writes a section title.
    /// </summary>
    /// <param name="sectionTitle">The section title</param>
    public virtual void WriteSectionTitle(string sectionTitle)
    {
        WriteOverflowingElement(HelpElementKind.SectionTitle, sectionTitle);
        WriteLine();
    }

    /// <summary>
    /// Writes a section ending.
    /// </summary>
    public virtual void WriteSectionEnd()
    {
        WriteLine();
    }

    /// <summary>
    /// Writes a usage clause with a single part.
    /// </summary>
    /// <param name="commandName">The command name</param>
    public virtual void WriteUsageClause(string commandName)
    {
        WriteIndentation();
        WriteOverflowingElement(HelpElementKind.CommandUsageName, commandName);
        WriteLine();
    }

    /// <summary>
    /// Writes a usage clause with a single token.
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="token">A usage token</param>
    public virtual void WriteUsageClause(string commandName, UsageToken token)
    {
        WriteIndentation();
        WriteOverflowingElement(HelpElementKind.CommandUsageName, commandName);
        HelpWriter.WriteWhiteSpace(1);
        WriteOverflowingElement(token.ElementKind, token.Text);
        WriteLine();
    }

    /// <summary>
    /// Writes a usage clause with two tokens.
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="firstToken">The first token</param>
    /// <param name="secondToken">The second token</param>
    public virtual void WriteUsageClause(string commandName, UsageToken firstToken, UsageToken secondToken)
    {
        WriteIndentation();
        WriteOverflowingElement(HelpElementKind.CommandUsageName, commandName);
        HelpWriter.WriteWhiteSpace(1);
        WriteOverflowingElement(firstToken.ElementKind, firstToken.Text);
        HelpWriter.WriteWhiteSpace(1);
        WriteOverflowingElement(secondToken.ElementKind, secondToken.Text);
        WriteLine();
    }

    /// <summary>
    /// Writes the command description section.
    /// </summary>
    /// <param name="sectionTitle">The section title</param>
    /// <param name="description">The command description</param>
    public virtual void WriteCommandDescriptionSection(string sectionTitle, string description)
    {
        WriteSectionTitle(sectionTitle);
        WriteParagraph(HelpElementKind.CommandDescription, description);
    }

    /// <summary>
    /// Writes an identifier list.
    /// </summary>
    /// <param name="listItems">List items to display.</param>
    public abstract void WriteListItems(HelpListItem[] listItems);

    /// <summary>
    /// Gets the help text writer.
    /// </summary>
    public IHelpTextWriter HelpWriter { get; }

    /// <summary>
    /// Gets the constrained column width.
    /// </summary>
    public int ColumnWidth { get; }

    /// <summary>
    /// Gets the indentation width.
    /// </summary>
    public int IndentationSpaces { get; }

    /// <summary>
    /// Writes one or more newline characters.
    /// </summary>
    /// <param name="count">Number of newline characters to write.</param>
    public void WriteLine(int count = 1) => HelpWriter.WriteLine(count);

    /// <summary>
    /// Writes an overflowing element, e.g. one that will be automatically wrapped by the display
    /// and not by the engine.
    /// </summary>
    /// <param name="elementKind">Kind of element to write.</param>
    /// <param name="value">Element content.</param>
    public void WriteOverflowingElement(HelpElementKind elementKind, string value)
    {
        HelpWriter.WriteElement(elementKind, value);
    }

    /// <summary>
    /// Writes a paragraph block of content.
    /// </summary>
    /// <param name="elementKind">The element kind.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="indentLevel">The level of indentation.</param>
    public void WriteParagraph(HelpElementKind elementKind, string content, int indentLevel = 1)
    {
        var indentationSpaces = IndentationSpaces * indentLevel;
        var blockWidth = ColumnWidth - indentationSpaces;

        for (var split = content.SplitLinesToLength(blockWidth);
             !split.IsEmpty;
             split = split.ResplitLinesToLength(blockWidth))
        {
            HelpWriter.WriteWhiteSpace(indentationSpaces);
            HelpWriter.WriteElement(elementKind, split.Left);
            HelpWriter.WriteLine();
        }
    }

    /// <summary>
    /// Writes whitespace characters that represent an indentation.
    /// </summary>
    public void WriteIndentation() => HelpWriter.WriteWhiteSpace(IndentationSpaces);
}