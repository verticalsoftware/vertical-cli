using Vertical.Cli.Diagnostics;
using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a special symbol for the help option.
/// </summary>
public sealed class UnboundSymbol : ICliSymbol
{
    internal UnboundSymbol(
        string identifier,
        AliasList aliasList,
        SpecialSymbolKind specialKind,
        HelpTopic? helpTopic)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        
        Identifier = identifier;
        SpecialKind = specialKind;
        Aliases = aliasList.GetValues() is { Length: > 0 } aliases
            ? aliases
            : throw Exceptions.EmptyUnboundSymbolAlias(nameof(aliasList));
        HelpTopic = helpTopic;
    }
    
    /// <summary>
    /// Gets the identifier of the symbol.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the unbound symbol kind.
    /// </summary>
    public SpecialSymbolKind SpecialKind { get; }

    /// <summary>
    /// Gets the aliases assigned to the help symbol.
    /// </summary>
    public string[] Aliases { get; }
    
    /// <inheritdoc />
    public HelpTopic? HelpTopic { get; init; }

    /// <inheritdoc />
    public string? GetRemarks() => HelpTopic?.Remarks;

    /// <inheritdoc />
    public IEnumerable<ExtendedRemarksSection> GetExtendedRemarksSections() => [];

    /// <inheritdoc />
    public string GetListIdentifier() => string.Join(", ", Aliases);

    /// <inheritdoc />
    public string? GetParameterName() => null;

    /// <inheritdoc />
    public SymbolKind Kind => SymbolKind.Option;

    /// <inheritdoc />
    public Arity Arity => Arity.ZeroOrOne;
}