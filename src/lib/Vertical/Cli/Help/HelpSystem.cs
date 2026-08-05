using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

/// <summary>
/// Manages the lifecycle of help system components.
/// </summary>
public static class HelpSystem
{
    /// <summary>
    /// Writes a help article to the console abstraction.
    /// </summary>
    /// <param name="configuration">The application configuration object.</param>
    /// <param name="targetCommand">The command to display help content for.</param>
    public static void WriteArticle(IRootConfigurationView configuration, Command targetCommand)
    {
        using var bufferedConsole = new BufferedConsole(configuration.Console);
        var cliSymbols = GetHelpSymbols(configuration, targetCommand);
        var helpOptions = configuration.HelpOptions;
        
        var articleInfo = new HelpEventInfo(cliSymbols)
        {
            Command = targetCommand,
            HelpOptionAliases = helpOptions.OptionAliases,
            HelpOptionRemarks = helpOptions.OptionRemarks,
            HelpProvider = helpOptions.HelpProvider,
            OutputWriter = new OutputWriter(bufferedConsole, configuration.OutputFormatter),
            DirectiveSymbols = configuration.GetDirectives()
        };

        helpOptions.ArticleWriter.WriteContent(articleInfo);
        bufferedConsole.Flush();
    }
    
    private static CliSymbol[] GetHelpSymbols(IRootConfigurationView configuration, Command command)
    {
        if (command.ModelType is null)
            return [];

        var modelConfiguration = configuration.GetModelConfiguration(command.ModelType);
        
        return modelConfiguration
            .BindingSources
            .OfType<CliSymbol>()
            .ToArray();
    }
}