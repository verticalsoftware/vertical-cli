namespace Vertical.Cli.Help;

/// <summary>
/// Defines help remarks.
/// </summary>
/// <param name="Title">Paragraph title</param>
/// <param name="Paragraphs">Paragraph content</param>
/// <param name="Placement">Placement where the content should be rendered</param>
public record HelpRemarks(string Title, string[] Paragraphs, HelpPlacement Placement = HelpPlacement.Bottom);