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
        var helpOptions = configuration.HelpOptions;
        
        var articleInfo = new HelpEventInfo(GetSymbols(configuration, targetCommand))
        {
            Command = targetCommand,
            HelpProvider = helpOptions.HelpProvider,
            OutputWriter = new OutputWriter(bufferedConsole, configuration.OutputFormatter),
        };

        helpOptions.ArticleWriter.WriteContent(articleInfo);
        bufferedConsole.Flush();
    }

    private static IEnumerable<ICliSymbol> GetSymbols(
        IRootConfigurationView configuration, 
        Command command)
    {
        var helpOptions = configuration.HelpOptions;

        if (command.ModelType is { } selectedModel)
        {
            var modelConfiguration = configuration.GetModelConfiguration(selectedModel);
            foreach (var symbol in modelConfiguration.BindingSources.OfType<CliSymbol>())
            {
                yield return symbol;
            }
        }

        foreach (var symbol in configuration.GetMiddlewareSymbols())
        {
            yield return symbol;
        }

        yield return helpOptions.CreateHelpSwitch();
    }
}