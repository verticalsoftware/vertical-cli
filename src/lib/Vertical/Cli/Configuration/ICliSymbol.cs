using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the basic data of a symbol.
/// </summary>
public interface ICliSymbol : IHelpSubject
{
    /// <summary>
    /// Gets the symbol kind.
    /// </summary>
    SymbolKind Kind { get; }
    
    /// <summary>
    /// Gets the symbol or parameter arity.
    /// </summary>
    Arity Arity { get; }
}