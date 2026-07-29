using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a directive symbol definition.
/// </summary>
public sealed class DirectiveSymbol : ICliSymbol
{
    internal DirectiveSymbol(
        string symbol,
        DirectiveParameterArity arity,
        Func<DirectiveEventInfo, Task> asyncHandler,
        SymbolHelpTopic? helpTopic)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        if (ArgumentSyntax.GetSyntaxKind(symbol) != SyntaxKind.None)
        {
            throw new ArgumentException($"Invalid directive symbol '{symbol}' (must be non-prefixed)");
        }
        ArgumentNullException.ThrowIfNull(asyncHandler);
        
        Symbol = symbol;
        Arity = arity;
        AsyncHandler = asyncHandler;
        HelpTopic = helpTopic;
    }
    
    /// <summary>
    /// Gets the symbol for the directive.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// Gets the parameter arity.
    /// </summary>
    public DirectiveParameterArity Arity { get; }

    /// <summary>
    /// Gets the asynchronous handler method.
    /// </summary>
    public Func<DirectiveEventInfo, Task> AsyncHandler { get; }

    /// <summary>
    /// Gets the help topic.
    /// </summary>
    public SymbolHelpTopic? HelpTopic { get; }
    
    HelpTopic? IHelpSubject.HelpTopic => HelpTopic;
    
    /// <inheritdoc />
    public override string ToString() => $"symbol={Symbol} (parameter={Arity}";

    /// <inheritdoc />
    public SymbolKind SymbolKind => SymbolKind.Directive;
}