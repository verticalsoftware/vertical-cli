using Vertical.Cli.Diagnostics;
using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a special symbol for the help option.
/// </summary>
public class UnboundSymbol : ICliSymbol
{
    internal UnboundSymbol(
        string identifier,
        AliasList aliasList,
        UnboundSymbolKind unboundKind,
        UnboundScope scope,
        HelpTopic? helpTopic)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        
        Identifier = identifier;
        UnboundKind = unboundKind;
        Scope = scope;
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
    public UnboundSymbolKind UnboundKind { get; }

    /// <summary>
    /// Gets the symbol's scope.
    /// </summary>
    public UnboundScope Scope { get; }

    /// <summary>
    /// Gets the aliases assigned to the help symbol.
    /// </summary>
    public string[] Aliases { get; }
    
    /// <inheritdoc />
    public HelpTopic? HelpTopic { get; init; }

    /// <inheritdoc />
    public HelpTopicKey HelpTopicKey => new("symbol", $"(Unbound).{Identifier}");

    /// <inheritdoc />
    public string? GetRemarks() => HelpTopic?.Remarks;

    /// <inheritdoc />
    public IEnumerable<ExtendedRemarksSection> GetExtendedRemarksSections() => [];

    /// <inheritdoc />
    public string GetListIdentifier() => string.Join(", ", Aliases);

    /// <inheritdoc />
    public string? GetParameterName() => null;

    /// <inheritdoc />
    public string DisplayName => string.Join(", ", Aliases);

    /// <inheritdoc />
    public SymbolKind Kind => SymbolKind.Option;

    /// <inheritdoc />
    public Arity Arity => Arity.ZeroOrOne;
}