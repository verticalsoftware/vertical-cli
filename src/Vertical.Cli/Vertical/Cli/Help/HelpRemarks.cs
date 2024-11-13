namespace Vertical.Cli.Help;

/// <summary>
/// Defines help remarks.
/// </summary>
/// <param name="Title">Paragraph title</param>
/// <param name="Paragraphs">Paragraph content</param>
public record HelpRemarks(string Title, string[] Paragraphs);