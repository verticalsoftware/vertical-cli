using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

public static class HelpSystem
{
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