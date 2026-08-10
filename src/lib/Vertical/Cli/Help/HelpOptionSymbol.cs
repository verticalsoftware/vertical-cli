using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents a special symbol for the help option.
/// </summary>
public sealed class HelpOptionSymbol : ICliSymbol
{
    /// <summary>
    /// Gets the aliases assigned to the help symbol.
    /// </summary>
    public required string[] Aliases { get; init; }
    
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