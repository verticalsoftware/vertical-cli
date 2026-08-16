using Vertical.Cli.Configuration;
using Vertical.Cli.IO;
using Vertical.Cli.Middleware;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents the data of a help article.
/// </summary>
public sealed class HelpEventInfo
{
    internal HelpEventInfo(IEnumerable<ICliSymbol> symbols)
    {
        Symbols = symbols.ToArray();
        
        PositionalSymbols = Symbols
            .Where(symbol => symbol.Kind == SymbolKind.PositionArgument)
            .Cast<CliSymbol>()
            .OrderBy(symbol => symbol.OrdinalPosition)
            .ToArray();
        
        NamedSymbols = Symbols
            .Where(symbol => symbol.Kind is SymbolKind.Option or SymbolKind.Switch)
            .ToArray();

        DirectiveSymbols = Symbols
            .Where(symbol => symbol.Kind == SymbolKind.Directive)
            .ToArray();
    }

    /// <summary>
    /// Gets the display width of the output writer.
    /// </summary>
    public int DisplayWidth => OutputWriter.DisplayWidth;

    /// <summary>
    /// Gets the system help symbol.
    /// </summary>
    public ICliSymbol HelpSymbol => Symbols.First(symbol => symbol.SystemKind == SystemKind.Help);

    /// <summary>
    /// Gets the command that is the subject of the article.
    /// </summary>
    public required Command Command { get; init; }
    
    /// <summary>
    /// Gets the symbols to display in the help topic.
    /// </summary>
    public IReadOnlyList<ICliSymbol> Symbols { get; }

    /// <summary>
    /// Gets the named symbols (options and switches).
    /// </summary>
    public IReadOnlyList<ICliSymbol> NamedSymbols { get; set; }

    /// <summary>
    /// Gets the argument symbols.
    /// </summary>
    public IReadOnlyList<CliSymbol> PositionalSymbols { get; }
    
    /// <summary>
    /// Gets the directive symbols.
    /// </summary>
    public IReadOnlyCollection<ICliSymbol> DirectiveSymbols { get; init; }
    
    /// <summary>
    /// Gets the output writer.
    /// </summary>
    public required OutputWriter OutputWriter { get; init; }
    
    /// <summary>
    /// Gets the help content provider.
    /// </summary>
    public required IHelpProvider HelpProvider { get; init; }
}