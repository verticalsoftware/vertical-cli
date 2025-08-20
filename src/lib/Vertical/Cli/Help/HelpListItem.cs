namespace Vertical.Cli.Help;

/// <summary>
/// Represents an item displayed in a list.
/// </summary>
/// <param name="Identifier">The name or aliases of the symbol.</param>
/// <param name="Description">The symbol description.</param>
/// <param name="ParameterSyntax">For options or switches, the name of the parameter syntax.</param>
public record HelpListItem(
    string Identifier,
    string? Description,
    string? ParameterSyntax = null);