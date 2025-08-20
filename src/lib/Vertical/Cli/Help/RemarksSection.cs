namespace Vertical.Cli.Help;

/// <summary>
/// Represents a remarks section.
/// </summary>
/// <param name="SectionTitle">Section title</param>
/// <param name="Content">Content to display</param>
public record RemarksSection(string SectionTitle, string Content);