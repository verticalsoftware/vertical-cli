using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an intercepting option symbol.
/// </summary>
public sealed class AncillaryOptionSymbol : ISymbol
{
    internal AncillaryOptionSymbol(
        AncillaryOptionKind kind,
        string[] aliases,
        SymbolHelpTag? helpTag)
    {
        Kind = kind;
        Aliases = aliases;
        HelpTag = helpTag;
    }

    /// <inheritdoc />
    public SymbolBehavior Behavior => SymbolBehavior.Option;

    /// <inheritdoc />
    public Arity Arity => Arity.ZeroOrOne;

    /// <summary>
    /// Gets the symbol kind.
    /// </summary>
    public AncillaryOptionKind Kind { get; }

    /// <inheritdoc />
    public string[] Aliases { get; }

    /// <inheritdoc />
    public int Precedence => 0;

    /// <inheritdoc />
    public SymbolHelpTag? HelpTag { get; }

    /// <inheritdoc />
    public override string ToString() => Aliases.Length switch
    {
        > 1 => $"[{string.Join(", ", Aliases)}]",
        _ => Aliases[0]
    };
}