namespace Vertical.Cli.Help;

/// <summary>
/// Represents help for a directive.
/// </summary>
/// <param name="UsageSyntax">The usage clause fpr the directive.</param>
/// <param name="Description">Optional description of the directive.</param>
public record DirectiveHelpTag(string UsageSyntax, string? Description);