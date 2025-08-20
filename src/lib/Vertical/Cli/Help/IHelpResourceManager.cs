using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents an object that provides help resource content.
/// </summary>
public interface IHelpResourceManager
{
    /// <summary>
    /// Gets a description of the command.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <returns><see cref="string"/> if there is a description resource available</returns>
    string? GetCommandDescription(ICommand command);

    /// <summary>
    /// Gets the description section title.
    /// </summary>
    string DescriptionSectionTitle { get; }

    /// <summary>
    /// Gets the usage section title.
    /// </summary>
    string UsageSectionTitle { get; }

    /// <summary>
    /// Gets the placeholder token that represents arguments and options of a subcommand.
    /// </summary>
    string SubCommandUsageArgumentsToken { get; }

    /// <summary>
    /// Gets the directive section title.
    /// </summary>
    string DirectiveSectionTitle { get; }

    /// <summary>
    /// Gets the placeholder token that represents a sub command name.
    /// </summary>
    /// <param name="subCommands"></param>
    /// <returns></returns>
    string GetSubCommandUsageToken(IReadOnlyList<ICommand> subCommands);

    /// <summary>
    /// Gets the placeholder token that represents one or more arguments in a usage clause.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <param name="symbols">Argument symbols</param>
    /// <returns><see cref="string"/></returns>
    string GetUsageArgumentToken(ICommand command, IReadOnlyList<ISymbol> symbols);

    /// <summary>
    /// Gets the placeholder token that represents one or more options in a usage clause.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <param name="symbols">Argument symbols</param>
    /// <returns><see cref="string"/></returns>
    string GetUsageOptionToken(ICommand command, IReadOnlyList<ISymbol> symbols);

    /// <summary>
    /// Formats a list of alias names for an option.
    /// </summary>
    /// <param name="symbol">The option or switch symbol.</param>
    /// <returns><see cref="string"/></returns>
    string GetOptionAliasList(ISymbol symbol);

    /// <summary>
    /// Gets the syntax of an argument's value.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    string GetArgumentParameterSyntax(ISymbol symbol);

    /// <summary>
    /// Gets the syntax of an option's parameter.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    string? GetOptionParameterSyntax(ISymbol symbol);

    /// <summary>
    /// Gets the groupings of a command list.
    /// </summary>
    /// <param name="commands">The sub-commands of the subject.</param>
    /// <returns>
    /// An enumeration of groupings where the key is the section title and the items are
    /// the commands in the desired display order.
    /// </returns>
    IEnumerable<IGrouping<string, ISubCommand>> GetCommandGroupings(IReadOnlyList<ISubCommand> commands);

    /// <summary>
    /// Gets the groupings of an argument symbol list.
    /// </summary>
    /// <param name="symbols">The symbols of the subject.</param>
    /// <returns>
    /// An enumeration of groupings where the key is the section title and the items are
    /// the symbols in the desired display order.
    /// </returns>
    IEnumerable<IGrouping<string, ISymbol>> GetArgumentSymbolGroupings(IEnumerable<ISymbol> symbols);

    /// <summary>
    /// Gets the groupings of an option symbol list.
    /// </summary>
    /// <param name="symbols">The symbols of the subject.</param>
    /// <returns>
    /// An enumeration of groupings where the key is the section title and the items are
    /// the symbols in the desired display order.
    /// </returns>
    IEnumerable<IGrouping<string, ISymbol>> GetOptionSymbolGroupings(IEnumerable<ISymbol> symbols);

    /// <summary>
    /// Gets the description for an argument, option, or switch.
    /// </summary>
    /// <param name="symbol">Symbol instance</param>
    /// <returns><see cref="string"/> or <c>null</c></returns>
    string? GetSymbolDescription(ISymbol symbol);

    /// <summary>
    /// Gets content written right after the description section.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <returns><see cref="string"/></returns>
    RemarksSection? GetCommandIntroductoryRemarks(ICommand command);

    /// <summary>
    /// Gets content written before the end of the document after all other sections.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <returns><see cref="string"/></returns>
    RemarksSection? GetCommandFinalRemarks(ICommand command);
}