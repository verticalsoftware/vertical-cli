using Vertical.Cli.Configuration;
using Vertical.Cli.Middleware;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents configuration options for the help system.
/// </summary>
public sealed class HelpSystemOptions
{
    /// <summary>
    /// Gets the aliases for the help 
    /// </summary>
    public AliasList SymbolAliases { get; set; } = ["--help", "-?"];

    /// <summary>
    /// Gets or sets the option help topic.
    /// </summary>
    public HelpTopic? SymbolHelpTopic { get; set; } = "Display help for the current command.";

    /// <summary>
    /// Gets or sets the article writer instance.
    /// </summary>
    public HelpArticleWriter ArticleWriter { get; set; } = new();

    /// <summary>
    /// Gets or sets the help provider.
    /// </summary>
    public IHelpProvider HelpProvider { get; set; } = new DefaultHelpProvider();

    internal MiddlewareSwitch CreateHelpSwitch()
    {
        return new MiddlewareSwitch(
                "Help",
                SymbolAliases.GetValues(),
                _ => Task.FromResult<int?>(0),
                SymbolHelpTopic)
            { SystemKind = SystemKind.Help };
    }
}