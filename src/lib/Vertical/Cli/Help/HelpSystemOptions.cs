using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents configuration options for the help system.
/// </summary>
public sealed class HelpSystemOptions
{
    /// <summary>
    /// Gets or sets the help option.
    /// </summary>
    public UnboundSymbol Symbol { get; set; } = new(
        identifier: "Help",
        ["--help", "-?"],
        SpecialSymbolKind.HelpSymbol,
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