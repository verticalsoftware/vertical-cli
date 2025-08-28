using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an option, argument, or switch.
/// </summary>
public interface ISymbol
{
    /// <summary>
    /// Gets the symbol's parsing behavior.
    /// </summary>
    SymbolBehavior Behavior { get; }
    
    /// <summary>
    /// Gets the symbol arity.
    /// </summary>
    Arity Arity { get; }
    
    /// <summary>
    /// Gets the symbol's aliases.
    /// </summary>
    string[] Aliases { get; }
    
    /// <summary>
    /// Gets an integer that determines the parse order for the symbol.
    /// </summary>
    int Precedence { get; }
    
    /// <summary>
    /// Gets an application defined help tag.
    /// </summary>
    SymbolHelpTag? HelpTag { get; }
}