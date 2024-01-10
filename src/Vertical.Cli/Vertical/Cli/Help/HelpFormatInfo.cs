using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

public class HelpFormatInfo
{
    internal HelpFormatInfo(
        ICommandDefinition command,
        IEnumerable<ICommandDefinition> commands,        
        IEnumerable<SymbolDefinition> symbols)
    {
        Subject = command;
        RootCommand = command.GetRootCommand();
        SubCommands = commands.ToArray();
        Symbols = symbols.ToArray();
        SymbolLookup = Symbols.ToLookup(symbol => symbol.Type);
    }

    public ILookup<SymbolType, SymbolDefinition> SymbolLookup { get; set; }

    public SymbolDefinition[] Symbols { get; }
    public ICommandDefinition[] SubCommands { get; }
    internal ICommandDefinition Subject { get; }
    internal ICommandDefinition RootCommand { get; }
}