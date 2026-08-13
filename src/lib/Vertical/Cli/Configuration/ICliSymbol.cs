using Vertical.Cli.Help;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the basic data of a symbol.
/// </summary>
public interface ICliSymbol : IHelpSubject
{
    /// <summary>
    /// Gets a name for display in diagnostics and other output. Does not include
    /// the symbol type.
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    /// Gets the symbol kind.
    /// </summary>
    SymbolKind Kind { get; }
    
    /// <summary>
    /// Gets the symbol or parameter arity.
    /// </summary>
    Arity Arity { get; }

    /// <summary>
    /// Gets the aliases for the symbol.
    /// </summary>
    string[] Aliases => [];
}