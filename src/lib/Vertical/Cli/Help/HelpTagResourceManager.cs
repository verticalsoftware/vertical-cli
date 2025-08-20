using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Internal;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents the standard help resource manager that uses content configured using
/// <see cref="CommandLineBuilder"/> and usage of the help tag property.
/// </summary>
public class HelpTagResourceManager : IHelpResourceManager
{
    /// <summary>
    /// Gets a description of the command.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <returns><see cref="string"/> if there is a description resource available</returns>
    public virtual string? GetCommandDescription(ICommand command)
    {
        return (command.HelpTag as CommandHelpTag)?.Description
               ?? command.HelpTag as string;
    }

    /// <summary>
    /// Gets content written right after the description section.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <returns><see cref="string"/></returns>
    public virtual RemarksSection? GetCommandIntroductoryRemarks(ICommand command)
    {
        return (command.HelpTag as CommandHelpTag)?.IntroductoryRemarks;
    }
    
    /// <summary>
    /// Gets content written before the end of the document after all other sections.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <returns><see cref="string"/></returns>
    public virtual RemarksSection? GetCommandFinalRemarks(ICommand command)
    {
        return (command.HelpTag as CommandHelpTag)?.FinalRemarks;
    }
    
    /// <summary>
    /// Gets the description section title.
    /// </summary>
    public virtual string DescriptionSectionTitle => "Description:";

    /// <summary>
    /// Gets the usage section title.
    /// </summary>
    public virtual string UsageSectionTitle => "Usage:";
    
    /// <summary>
    /// Gets the placeholder token that represents arguments and options of a subcommand.
    /// </summary>
    public virtual string SubCommandUsageArgumentsToken => "[arguments, options...]";

    /// <inheritdoc />
    public string DirectiveSectionTitle => "Directives:";

    /// <summary>
    /// Gets the placeholder token that represents a sub command name.
    /// </summary>
    /// <param name="subCommands"></param>
    /// <returns></returns>
    public virtual string GetSubCommandUsageToken(IReadOnlyList<ICommand> subCommands) => "<command>";
    
    /// <summary>
    /// Gets the placeholder token that represents one or more arguments in a usage clause.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <param name="symbols">Argument symbols</param>
    /// <returns><see cref="string"/></returns>
    public virtual string GetUsageArgumentToken(ICommand command, IReadOnlyList<ISymbol> symbols)
    {
        return symbols.Count switch
        {
            1 when symbols[0].Arity.MinCount == 1 => $"<{GetArgumentParameterSyntax(symbols[0])}>",
            1 => $"[{GetArgumentParameterSyntax(symbols[0])}]",
            { } when symbols.Any(arg => arg.Arity.MinCount == 1) => "<arguments>",
            _ => "[arguments]"
        };
    }
    
    /// <summary>
    /// Gets the placeholder token that represents one or more options in a usage clause.
    /// </summary>
    /// <param name="command">Command subject</param>
    /// <param name="symbols">Argument symbols</param>
    /// <returns><see cref="string"/></returns>
    public virtual string GetUsageOptionToken(ICommand command, IReadOnlyList<ISymbol> symbols)
    {
        return symbols.Count switch
        {
            1 => $"[{GetOptionAliasList(symbols[0])}]",
            _ => "[options]"
        };
    }

    /// <summary>
    /// Formats a list of alias names for an option.
    /// </summary>
    /// <param name="symbol">The option or switch symbol.</param>
    /// <returns><see cref="string"/></returns>
    public virtual string GetOptionAliasList(ISymbol symbol)
    {
        return symbol.Aliases.Length == 1 ? symbol.Aliases[0] : string.Join(", ", symbol.Aliases);
    }

    /// <summary>
    /// Gets the syntax of an argument's value.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns>Formatted argument value name..</returns>
    public virtual string GetArgumentParameterSyntax(ISymbol symbol)
    {
        if (symbol.HelpTag is SymbolHelpTag { ParameterSyntax.Length: > 0 } helpTag)
            return helpTag.ParameterSyntax;
        
        var name = symbol.Aliases[0];
        
        return symbol.Arity.MaxCount is null ? $"{name} [...]" : name;
    }

    /// <summary>
    /// Gets the syntax of an options's value.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns>Formatted parameter name..</returns>
    public string? GetOptionParameterSyntax(ISymbol symbol)
    {
        if (symbol is not IPropertyBinding propertyBinding)
            return null;

        if (symbol.HelpTag is SymbolHelpTag { ParameterSyntax.Length: > 0 } helpTag)
            return helpTag.ParameterSyntax;

        var inferredName = propertyBinding.BindingName.ToUpperSnakeCase();
        return $"<{inferredName}>";
    }

    /// <summary>
    /// Gets the groupings of a command list.
    /// </summary>
    /// <param name="commands">The sub-commands of the subject.</param>
    /// <returns>
    /// An enumeration of groupings where the key is the section title and the items are
    /// the commands in the desired display order.
    /// </returns>
    public virtual IEnumerable<IGrouping<string, ISubCommand>> GetCommandGroupings(IReadOnlyList<ISubCommand> commands)
    {
        return commands.GroupBy(_ => "Commands:");
    }

    /// <summary>
    /// Gets the groupings of an argument symbol list.
    /// </summary>
    /// <param name="symbols">The symbols of the subject.</param>
    /// <returns>
    /// An enumeration of groupings where the key is the section title and the items are
    /// the symbols in the desired display order.
    /// </returns>
    public virtual IEnumerable<IGrouping<string, ISymbol>> GetArgumentSymbolGroupings(IEnumerable<ISymbol> symbols)
    {
        return symbols.GroupBy(_ => "Arguments:");
    }
    
    /// <summary>
    /// Gets the groupings of an option symbol list.
    /// </summary>
    /// <param name="symbols">The symbols of the subject.</param>
    /// <returns>
    /// An enumeration of groupings where the key is the section title and the items are
    /// the symbols in the desired display order.
    /// </returns>
    public virtual IEnumerable<IGrouping<string, ISymbol>> GetOptionSymbolGroupings(IEnumerable<ISymbol> symbols)
    {
        return symbols.GroupBy(symbol => symbol is AncillaryOptionSymbol
            ? "Global options:"
            : "Options:");
    }

    /// <summary>
    /// Gets the description for an argument, option, or switch.
    /// </summary>
    /// <param name="symbol">Symbol instance</param>
    /// <returns><see cref="string"/> or <c>null</c></returns>
    public virtual string? GetSymbolDescription(ISymbol symbol)
    {
        return symbol.HelpTag switch
        {
            SymbolHelpTag helpTag => helpTag.Description,
            _ => symbol.HelpTag as string
        };
    }
}