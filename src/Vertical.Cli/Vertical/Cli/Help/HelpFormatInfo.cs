using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Contains data used to render help content.
/// </summary>
public sealed class HelpFormatInfo
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

    /// <summary>
    /// Gets a lookup of symbols by type.
    /// </summary>
    public ILookup<SymbolType, SymbolDefinition> SymbolLookup { get; set; }

    /// <summary>
    /// Gets all symbols of the subject command.
    /// </summary>
    public SymbolDefinition[] Symbols { get; }
    
    /// <summary>
    /// Gets the sub command for the subject.
    /// </summary>
    public ICommandDefinition[] SubCommands { get; }
    
    /// <summary>
    /// Gets the subject command.
    /// </summary>
    internal ICommandDefinition Subject { get; }
    
    /// <summary>
    /// Gets the root command.
    /// </summary>
    internal ICommandDefinition RootCommand { get; }
}