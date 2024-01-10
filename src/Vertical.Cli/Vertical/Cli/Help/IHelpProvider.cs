using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Determines help content.
/// </summary>
public interface IHelpProvider
{
    /// <summary>
    /// Gets the description of a command.
    /// </summary>
    /// <param name="command">The subject command.</param>
    /// <returns>A string description or <c>null</c>.</returns>
    string? GetCommandDescription(ICommandDefinition command);

    /// <summary>
    /// Gets the name of a command.
    /// </summary>
    /// <param name="command">The subject command.</param>
    /// <returns>The command name.</returns>
    string GetCommandName(ICommandDefinition command);

    /// <summary>
    /// Returns whether a symbol is available in help content.
    /// </summary>
    Predicate<SymbolDefinition> SymbolSelector { get; }
    
    /// <summary>
    /// Returns whether a command is available in help content.
    /// </summary>
    Predicate<ICommandDefinition> CommandSelector { get; }

    /// <summary>
    /// Gets the grammar to display in a usage clause regarding command usage..
    /// </summary>
    /// <param name="command">The subject command.</param>
    /// <param name="subCommands">The sub commands of the subject.</param>
    /// <returns>The string to display in the usage clause that represent sub-commands.</returns>
    /// <remarks>
    /// E.g. dotnet &lt;command&gt; 
    /// </remarks>
    string? GetCommandUsageGrammar(ICommandDefinition command, IReadOnlyCollection<ICommandDefinition> subCommands);

    /// <summary>
    /// Gets the grammar to display in a usage clause regarding argument usage.
    /// </summary>
    /// <param name="command">The subject command.</param>
    /// <param name="symbols">The argument symbols of the subject.</param>
    /// <returns>The string to display in the usage clause that represent argument usage.</returns>
    /// <remarks>
    /// E.g. dotnet &lt;command&gt; arguments
    /// </remarks>
    string? GetArgumentsUsageGrammar(ICommandDefinition command, IEnumerable<SymbolDefinition> symbols);

    /// <summary>
    /// Gets the grammar to display in a usage clause regarding option usage.
    /// </summary>
    /// <param name="command">The subject command.</param>
    /// <param name="symbols">The option or switch symbols of the subject.</param>
    /// <returns>The string to display in the usage clause that represent argument usage.</returns>
    /// <remarks>
    /// E.g. dotnet &lt;command&gt; options
    /// </remarks>
    string? GetOptionsUsageGrammar(ICommandDefinition command, IEnumerable<SymbolDefinition> symbols);

    /// <summary>
    /// Gets the description of a symbol.
    /// </summary>
    /// <param name="symbol">The symbol being displayed.</param>
    /// <returns>A description for the argument, option, or switch.</returns>
    string? GetSymbolDescription(SymbolDefinition symbol);

    /// <summary>
    /// Gets the grammar for a symbol.
    /// </summary>
    /// <param name="symbol">The symbol being displayed.</param>
    /// <returns>The grammar that represents the symbols calling convention.</returns>
    /// <remarks>
    /// Symbol grammar typically includes the identifier, aliases, an argument name if applicable,
    /// and an ellipses to indicate the option or argument can be provided more than once.
    /// E.g. --path, -p &lt;PATH&gt;
    /// </remarks>
    string GetSymbolGrammar(SymbolDefinition symbol);

    /// <summary>
    /// Gets the key used to sort symbols in an argument or options list.
    /// </summary>
    /// <param name="symbol">The symbol being sorted.</param>
    /// <returns>The sort key.</returns>
    string GetSymbolSortKey(SymbolDefinition symbol);

    /// <summary>
    /// Gets the argument name to display within the symbol's grammar.
    /// </summary>
    /// <param name="symbol">The option symbol.</param>
    /// <returns>The argument name to display.</returns>
    /// <remarks>
    /// Symbol grammar typically includes the identifier, aliases, an argument name if applicable,
    /// and an ellipses to indicate the option or argument can be provided more than once.
    /// E.g. --path, -p &lt;PATH&gt;
    /// </remarks>
    string? GetSymbolArgumentName(SymbolDefinition symbol);
}