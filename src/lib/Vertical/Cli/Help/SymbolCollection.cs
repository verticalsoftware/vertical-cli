using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents a collection of symbols.
/// </summary>
/// <param name="ArgumentSymbols">Argument symbols.</param>
/// <param name="OptionSymbols">Option or switch symbols.</param>
/// <param name="AncillarySymbols">Intercepting help symbols.</param>
/// <param name="DirectiveHelpTags">Directive help tags.</param>
public record class SymbolCollection(
    IReadOnlyList<ISymbol> ArgumentSymbols,
    IReadOnlyList<ISymbol> OptionSymbols,
    IReadOnlyList<AncillaryOptionSymbol> AncillarySymbols,
    IReadOnlyList<DirectiveHelpTag> DirectiveHelpTags);