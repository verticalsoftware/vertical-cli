namespace Vertical.Cli.Help;

/// <summary>
/// Defines structured help content for 
/// </summary>
/// <param name="Description">A description of the symbol.</param>
/// <param name="ParameterSyntax">The syntax that describes the symbol's value parameter.</param>
public record SymbolHelpTag(string Description, string? ParameterSyntax);