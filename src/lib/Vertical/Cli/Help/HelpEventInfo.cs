using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents the data of a help article.
/// </summary>
public sealed class HelpEventInfo
{
    internal HelpEventInfo(IEnumerable<CliSymbol> symbols)
    {
        Symbols = symbols.ToArray();
        
        PositionalSymbols = Symbols
            .Where(symbol => symbol.Kind == SymbolKind.PositionArgument)
            .OrderBy(symbol => symbol.OrdinalPosition)
            .ToArray();
        
        NamedSymbols = Symbols
            .Where(symbol => symbol.Kind is SymbolKind.Option or SymbolKind.Switch)
            .ToArray();
    }

    /// <summary>
    /// Gets the display width of the output writer.
    /// </summary>
    public int DisplayWidth => OutputWriter.DisplayWidth;

    /// <summary>
    /// Gets the command that is the subject of the article.
    /// </summary>
    public required Command Command { get; init; }

    /// <summary>
    /// Gets the help option symbol.
    /// </summary>
    public required IReadOnlyList<UnboundSymbol> UnboundSymbols { get; init; }

    /// <summary>
    /// Gets the help option.
    /// </summary>
    public UnboundSymbol Help => UnboundSymbols.First(symbol => 
        symbol.UnboundKind == UnboundSymbolKind.HelpSymbol);
    
    /// <summary>
    /// Gets the symbols to display in the help topic.
    /// </summary>
    public IReadOnlyList<CliSymbol> Symbols { get; }

    /// <summary>
    /// Gets the named symbols (options and switches).
    /// </summary>
    public IReadOnlyList<CliSymbol> NamedSymbols { get; set; }

    /// <summary>
    /// Gets the argument symbols.
    /// </summary>
    public IReadOnlyList<CliSymbol> PositionalSymbols { get; }
    
    /// <summary>
    /// Gets the directive symbols.
    /// </summary>
    public required IReadOnlyCollection<IDirectiveSymbol> DirectiveSymbols { get; init; }
    
    /// <summary>
    /// Gets the output writer.
    /// </summary>
    public required OutputWriter OutputWriter { get; init; }
    
    /// <summary>
    /// Gets the help content provider.
    /// </summary>
    public required IHelpProvider HelpProvider { get; init; }
}