using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents configuration options for the help system.
/// </summary>
public sealed class HelpSystemOptions
{
    /// <summary>
    /// Gets the aliases for the help 
    /// </summary>
    public AliasList OptionAliases
    {
        get => [..Symbol.Aliases];
        set => Symbol = new UnboundSymbol(
            Symbol.Identifier,
            value,
            UnboundSymbolKind.HelpSymbol,
            UnboundScope.Global,
            Symbol.HelpTopic);
    }

    /// <summary>
    /// Gets or sets the option help topic.
    /// </summary>
    public HelpTopic? OptionHelpTopic
    {
        get => Symbol.HelpTopic;
        set => Symbol = new UnboundSymbol(
            Symbol.Identifier,
            OptionAliases,
            Symbol.UnboundKind,
            Symbol.Scope,
            value);
    }
    
    /// <summary>
    /// Gets or sets the help option.
    /// </summary>
    internal UnboundSymbol Symbol { get; private set; } = new(
        identifier: "Help",
        ["--help", "-?"],
        UnboundSymbolKind.HelpSymbol,
        UnboundScope.Global,
        "Displays help for the current command.");

    /// <summary>
    /// Gets or sets the article writer instance.
    /// </summary>
    public HelpArticleWriter ArticleWriter { get; set; } = new();

    /// <summary>
    /// Gets or sets the help provider.
    /// </summary>
    public IHelpProvider HelpProvider { get; set; } = new DefaultHelpProvider();
}